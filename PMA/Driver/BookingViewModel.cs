using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Windows.Input;
using PMA.Modules;
using Mysqlx.Crud;

namespace PMA.Driver
{
    public class BookingViewModel : INotifyPropertyChanged
    {
        Server server = new Server();

        public ObservableCollection<Book> Bookings { get; set; } = new ObservableCollection<Book>();
        private String _lastlogged;
        public String LastLogged
        {
            get => _lastlogged;
            set
            {
                _lastlogged = value;
                OnPropertyChanged(nameof(LastLogged));
            }
        }

        public ICommand ConfirmCommand { get; }
        public ICommand DeclineCommand { get; }

        private Timer _refresh;
        private Timer _refreshStat;
        private Timer _resetStat;
        private Timer _refreshLastLogged;

        public BookingViewModel()
        {
            ConfirmCommand = new Command<Book>(ConfirmBooking);
            DeclineCommand = new Command<Book>(DeclineBooking);

            LoadBookings();
            UpdateStatus();
            ResetDriverAvailability();
            GetLastLogged();
            _refresh = new Timer(_ => LoadBookings(), null, TimeSpan.Zero, TimeSpan.FromSeconds(2));
            _refreshStat = new Timer(_ => UpdateStatus(), null, TimeSpan.Zero, TimeSpan.FromSeconds(2));
            _resetStat = new Timer(_ => ResetDriverAvailability(), null, TimeSpan.Zero, TimeSpan.FromSeconds(2));
            _refreshLastLogged = new Timer(_ => GetLastLogged(), null, TimeSpan.Zero, TimeSpan.FromSeconds(2));
        }

        public void UpdateStatus()
        {
            using (var conn = new MySqlConnection(server.server))
            {
                conn.Open();
                using (var cmd = new MySqlCommand(@"
                                                    UPDATE activedrivers 
                                    SET status = 'booked'
                                    WHERE AssignedSeats = (
                                        SELECT capacity FROM tricycles 
                                        WHERE tricycles.id = activedrivers.TricycleId
                                    );", conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
            
        }

        public void GetLastLogged()
        {
            String query = @"SELECT drivers.LastName, drivers.FirstName FROM activedrivers
                            JOIN drivers ON activedrivers.DriverId=drivers.DriverId
                            WHERE DATE(activedrivers.LoginDT)=CURDATE()
                            ORDER BY LoginDT DESC
                            LIMIT 1";
            using (var conn = new MySqlConnection(server.server))
            {
                conn.Open();
                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.Clear();
                    using (var rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            LastLogged = $"{rdr.GetString("LastName").ToUpper()}, {rdr.GetString("FirstName").ToUpper()}";
                        }
                    }
                }
            }
        }

        public void ResetDriverAvailability()
        {
            server.Connect();
            using (var cmd = new MySqlCommand())
            {
                cmd.Connection = server.connection;
                cmd.CommandText = @"SELECT DriverId
                                    FROM bookingtransactions
                                    WHERE DATE(BookingDT) = CURDATE()
                                    GROUP BY DriverId 
                                    HAVING SUM(CASE WHEN status NOT IN ('completed', 'canceled') THEN 1 ELSE 0 END) = 0;";

                using (var rdr = cmd.ExecuteReader())
                {
                    var completedDriverIds = new List<string>();
                    while (rdr.Read())
                    {
                        completedDriverIds.Add(rdr.GetString("DriverId"));
                    }

                    rdr.Close(); // Must close reader before issuing new commands on the same connection

                    foreach (var driverId in completedDriverIds)
                    {
                        using (var updateCmd = new MySqlCommand(@"UPDATE activedrivers 
                                                          SET AssignedSeats = 0, status = 'available', trips = trips +1 
                                                          WHERE DriverId = @driverid", server.connection))
                        {
                            updateCmd.Parameters.AddWithValue("@driverid", driverId);
                            updateCmd.ExecuteNonQuery();
                        }
                    }
                }
            }
        }


        private void LoadBookings()
        {
            var newBookings = new List<Book>();
            using (var conn = new MySqlConnection(server.server))
            {
                conn.Open();
                using (var cmd = new MySqlCommand(@"SELECT 
                                                users.LastName AS ulname, users.FirstName AS ufname, users.MiddleInitial, users.NameExtension, 
                                                drivers.LastName AS dlname, drivers.FirstName AS dfname, drivers.MiddleInitial, drivers.NameExtension, 
                                                bookingtransactions.DriverId,
                                                bookingtransactions.location AS location,
                                                bookingtransactions.seats AS seats,
                                                bookingtransactions.status,
                                                bookingtransactions.BookingDT,
                                                bookingtransactions.id AS BookingId
                                                FROM bookingtransactions
                                                JOIN drivers ON drivers.DriverId = bookingtransactions.DriverId
                                                JOIN users ON users.UserId = bookingtransactions.UserId
                                                WHERE DATE(bookingtransactions.BookingDT) = CURDATE() AND bookingtransactions.status='pending';", conn))
                {
                    using (var rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            newBookings.Add(new Book
                            {
                                BookingId = rdr.GetInt32("BookingId"),
                                DriverName = $"{rdr.GetString("dlname")}, {rdr.GetString("dfname")}",
                                Location = rdr.GetString("location"),
                                BookedSeats = rdr.GetInt32("seats"),
                                PassengerName = $"{rdr.GetString("ulname")}, {rdr.GetString("ufname")}"
                            });
                        }
                    }
                }
            }

            App.Current.Dispatcher.Dispatch(() =>
            {
                // Remove bookings not in new list
                var toRemove = Bookings.Where(b => !newBookings.Any(nb => nb.BookingId == b.BookingId)).ToList();
                foreach (var item in toRemove)
                    Bookings.Remove(item);

                // Add or update bookings
                foreach (var newBooking in newBookings)
                {
                    var existing = Bookings.FirstOrDefault(b => b.BookingId == newBooking.BookingId);
                    if (existing == null)
                    {
                        Bookings.Add(newBooking);
                    }
                    else
                    {
                        // Update values if needed
                        existing.DriverName = newBooking.DriverName;
                        existing.Location = newBooking.Location;
                        existing.BookedSeats = newBooking.BookedSeats;
                        existing.PassengerName = newBooking.PassengerName;
                        // Raise property changed if you're not using a full model binding
                    }
                }
            });
        }


        private void ConfirmBooking(Book booking)
        {
            UpdateBookingStatus(booking.BookingId, "ongoing");
            booking.Status = "ongoing";
            OnPropertyChanged(nameof(Bookings));
            LoadBookings();
            UpdateAssignedSeats(booking.DriverId, booking.BookedSeats);
        }

        private void DeclineBooking(Book booking)
        {
            UpdateBookingStatus(booking.BookingId, "rejected");
            booking.Status = "rejected";
            OnPropertyChanged(nameof(Bookings));
            LoadBookings();
        }

        public void UpdateAssignedSeats(String driverid, int booked)
        {
            using (var conn = new MySqlConnection(server.server))
            {
                conn.Open();
                using (var cmd = new MySqlCommand(@"UPDATE activedrivers
                            SET AssignedSeats = AssignedSeats + @bookedSeats WHERE DriverId = @BookedDriverId;", conn))
                {
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@bookedSeats", booked);
                    cmd.Parameters.AddWithValue("@BookedDriverId", driverid);
                    cmd.ExecuteNonQuery();
                }
                
            }
        }



        private void UpdateBookingStatus(int bookingId, string newStatus)
        {
            var server = new Server();
            server.Connect();

            using (var cmd = new MySqlCommand("UPDATE bookingtransactions SET status=@status WHERE id=@id", server.connection))
            {
                cmd.Parameters.AddWithValue("@status", newStatus);
                cmd.Parameters.AddWithValue("@id", bookingId);
                cmd.ExecuteNonQuery();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
