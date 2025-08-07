using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using PickMeAppV2.Modules;

namespace PickMeAppV2.DriverPages
{
    public class DriverModelService : INotifyPropertyChanged
    {
        ServerConnection server = new ServerConnection();

        private Timer _refresh;

        public ObservableCollection<Book> Bookings { get; set; } = new ObservableCollection<Book>();

        public ICommand ConfirmBook { get; set; }
        public ICommand DeclineBook { get; set; }

        public DriverModelService()
        {
            BookingCommands();
            refresh();
        }

        public void refresh()
        {
            _refresh = new Timer(_ =>
            {
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    ResetDriverAvailability();
                    UpdateDriverStat();
                });
            }, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
        }

        private void BookingCommands()
        {
            ConfirmBook = new Command<Book>(async (book) =>
            {
                bool confirm = await Application.Current.MainPage.DisplayAlert("CONFIRM BOOKING", $"Are you sure you want to confirm the request of {book.PassengerName}?", "YES", "NO");

                if (!confirm) return;

                bool confirmed = UpdateBookingStat(book.BookingId, "ongoing");

                if (confirmed)
                {
                    await Application.Current.MainPage.DisplayAlert("DEBUG", "Updated", "OK");
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("DEBUG", "Something went wrong.", "OK");
                }

            });

            DeclineBook = new Command<Book>(async (book) =>
            {
                bool confirm = await Application.Current.MainPage.DisplayAlert("DECLINE BOOKING", $"Are you sure you want to decline the request of {book.PassengerName}?", "YES", "NO");

                if (!confirm) return;

                bool declined = UpdateBookingStat(book.BookingId, "declined");

                if (declined)
                {
                    // say something
                }
                else
                {
                    // say something
                }

            });
        }

        // refresh to get bookings in real-time
        public void LoadBookings()
        {
            var books = GetBookings();
            Bookings.Clear();
            foreach (var book in books)
            {
                Bookings.Add(book);
            }
        }

        public List<Book> GetBookings()
        {
            var books = new List<Book>();
            String query = @"SELECT b.id AS bookingid, b.seats AS seats, b.bookingDT AS bookingDT, b.status AS stat,
                            d.lastname AS dlname, d.firstname AS dfname, d.middleinitial AS dmi, d.extension AS dex,
                            u.lastname AS ulname, u.firstname AS ufname, u.middleinitial AS umi, u.extension AS uex,
                            l.name AS location FROM bookingtransactions b
                            JOIN drivers d ON b.bookeddriver=d.tagid
                            JOIN users u ON b.passenger=u.userid
                            JOIN locations l ON b.location=l.id
                            WHERE DATE(b.bookingDT)=CURDATE() AND b.status='pending'";

            using var conn = server.GetConnection();
            conn.Open();

            using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.Clear();

            using var rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                String BookedDriver = $"{rdr.GetString("dlname")}, {rdr.GetString("dfname")} {rdr.GetString("dmi")}. {(rdr["dex"] != DBNull.Value ? rdr.GetString("dex") : "")}";
                String Passenger = $"{rdr.GetString("ulname")}, {rdr.GetString("ufname")} {rdr.GetString("umi")}. {(rdr["uex"] != DBNull.Value ? rdr.GetString("uex") : "")}";
                books.Add(new Book
                {
                    BookingId = rdr.GetInt32("bookingid"),
                    BookedDriverName = BookedDriver.ToUpper(),
                    PassengerName = Passenger.ToUpper(),
                    BookedSeats = rdr.GetInt32("seats"),
                    LocationName = rdr.GetString("location").ToUpper()
                });
            }

            return books;
        }

        private bool UpdateBookingStat(int BookingId, String ConfirmationStat)
        {
            int affected = 0;
            String query = "UPDATE bookingtransactions SET status=@stat WHERE id=@id";

            using var conn = server.GetConnection();
            conn.Open();

            using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@stat", ConfirmationStat);
            cmd.Parameters.AddWithValue("@id", BookingId);
            affected = cmd.ExecuteNonQuery();
            return affected > 0 ? true : false;
        }

        // refresh this part
        public void UpdateDriverStat()
        {
            String query = @"UPDATE activedrivers ad JOIN drivers d ON ad.driverid=d.tagid
                            SET ad.status = CASE 
                            WHEN assignedseats = d.capacity THEN 'booked'
                            ELSE 'available'
                            END
                            WHERE DATE(ad.loginDT)=CURDATE()";

            using var conn = server.GetConnection();
            conn.Open();

            using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.Clear();
            cmd.ExecuteNonQuery();
        }

        public void ResetDriverAvailability()
        {
            String query = @"SELECT bt.bookeddriver, MAX(bt.BookingDT) AS lastBookingDT
                            FROM bookingtransactions bt
                            WHERE DATE(bt.BookingDT) = CURDATE()
                            GROUP BY bt.bookeddriver
                            HAVING SUM(CASE WHEN bt.status NOT IN ('completed', 'canceled') THEN 1 ELSE 0 END) = 0";

            String update = @" UPDATE activedrivers 
                            SET AssignedSeats = 0, status = 'available', completedbookings = completedbookings + 1, lastResetDT = NOW()
                            WHERE driverid = @driverid AND (lastResetDT IS NULL OR @lastBookingDT > lastResetDT)";

            using var conn = server.GetConnection();
            conn.Open();

            using var cmd = new MySqlCommand(query, conn);
            using var rdr = cmd.ExecuteReader();

            var driversToReset = new List<(string driverId, DateTime lastBookingDT)>();

            while (rdr.Read())
            {
                string driverId = rdr.GetString("bookeddriver");
                DateTime lastBookingDT = rdr.GetDateTime("lastBookingDT");
                driversToReset.Add((driverId, lastBookingDT));
            }

            rdr.Close();

            foreach (var (driverId, lastBookingDT) in driversToReset)
            {
                using var updateCmd = new MySqlCommand(update, conn);
                updateCmd.Parameters.AddWithValue("@driverid", driverId);
                updateCmd.Parameters.AddWithValue("@lastBookingDT", lastBookingDT);
                updateCmd.ExecuteNonQuery();
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
