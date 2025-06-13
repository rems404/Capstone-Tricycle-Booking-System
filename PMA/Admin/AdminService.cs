using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using PMA.Modules;

namespace PMA.Admin
{
    public class AdminService : INotifyPropertyChanged
    {
        Server server = new Server();
        MySqlCommand cmd = new MySqlCommand();

        public ObservableCollection<Drivers> DriverList { get; set; } = new ObservableCollection<Drivers>();
        public ObservableCollection<Book> BookingList { get; set; } = new ObservableCollection<Book>();

        private int _dcount;
        public int DCount
        {
            get => _dcount;
            set
            {
                _dcount = value;
                OnPropertyChanged(nameof(DCount));
            }
        }

        private int _availabled;
        public int AvailableDriver
        {
            get => _availabled;
            set
            {
                _availabled = value;
                OnPropertyChanged(nameof(AvailableDriver));
            }
        }

        private int _bookeddriver;
        public int BookedDriver
        {
            get => _bookeddriver;
            set
            {
                _bookeddriver = value;
                OnPropertyChanged(nameof(BookedDriver));
            }
        }

        private int _pendingbooking;
        public int Pending
        {
            get => _pendingbooking;
            set
            {
                _pendingbooking = value;
                OnPropertyChanged(nameof(Pending));
            }
        }

        private int _canceledbooking;
        public int Canceled
        {
            get => _canceledbooking;
            set
            {
                _canceledbooking = value;
                OnPropertyChanged(nameof(Canceled));
            }
        }

        private int _completedbooking;
        public int Completed
        {
            get => _completedbooking;
            set
            {
                _completedbooking = value;
                OnPropertyChanged(nameof(Completed));
            }
        }

        private int _ongoingbooking;
        public int Ongoing
        {
            get => _ongoingbooking;
            set
            {
                _ongoingbooking = value;
                OnPropertyChanged(nameof(Ongoing));
            }
        }

        public void LoadaCurrentBookings()
        {
            var book = new List<Book>();
            server.Connect();
            cmd.Connection = server.connection;
            cmd.CommandText = @"SELECT drivers.LastName AS dlname, drivers.FirstName AS dfname, drivers.MiddleInitial AS dmi, drivers.NameExtension AS dex,
                                users.LastName AS ulname, users.FirstName AS ufname, users.MiddleInitial AS umi, users.NameExtension AS uex,
                                location, seats, BookingDT, status
                                FROM bookingtransactions 
                                JOIN drivers ON bookingtransactions.DriverId=drivers.DriverId
                                JOIN users ON bookingtransactions.UserId=users.UserId
                                WHERE DATE(BookingDT)=CURDATE()
                                ORDER BY BookingDT DESC;";
            using (cmd)
            {
                cmd.Parameters.Clear();
                using (var rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        var nameExtensionObj = rdr["dex"];
                        String dex = nameExtensionObj == DBNull.Value ? "" : nameExtensionObj.ToString().ToUpper();

                        var nameexuser = rdr["uex"];
                        String uex = nameExtensionObj == DBNull.Value ? "" : nameexuser.ToString().ToUpper();
                        book.Add(new Book
                        {
                            DriverName = $"{rdr.GetString("dlname").ToUpper()}, {rdr.GetString("dfname").ToUpper()} {rdr.GetString("dmi").ToUpper()} {dex}",
                            PassengerName = $"{rdr.GetString("ulname").ToUpper()}, {rdr.GetString("ufname").ToUpper()} {rdr.GetString("umi").ToUpper()} {uex}",
                            Location = rdr.GetString("location"),
                            BookedSeats = rdr.GetInt32("seats"),
                            BookingDT = rdr.GetDateTime("BookingDT"),
                            Status = rdr.GetString("status")
                        });
                    }
                }
            }
            BookingList.Clear();
            foreach (var b in book)
            {
                BookingList.Add(b);
            }
        }

        public void LoadSearchDriver(String searchval)
        {
            String query = @"SELECT * FROM drivers WHERE CONCAT(LastName, FirstName) LIKE @searchval";

            var drivers = new List<Drivers>();
            using (var conn = new MySqlConnection(server.server))
            {
                conn.Open();
                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@searchval", "%" + searchval + "%");
                    using (var rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            drivers.Add(new Drivers
                            {
                                DriverId = rdr.GetString("DriverId"),
                                LastName = rdr.GetString("LastName"),
                                FirstName = rdr.GetString("FirstName"),
                                MiddleInitial = rdr.GetString("MiddleInitial"),
                                Extension = rdr.IsDBNull("NameExtension") ? string.Empty : rdr.GetString("NameExtension"),
                                Address = rdr.GetString("address"),
                                Phone = rdr.GetString("phone")
                            });
                        }
                    }
                }
            }
            DriverList.Clear();
            foreach (var d in drivers)
            {
                DriverList.Add(d);
            }
        }

        public void LoadDriver()
        {
            var drivers = new List<Drivers>();
            server.Connect();
            cmd.Connection = server.connection;
            cmd.CommandText = @"SELECT * FROM drivers;";
            using (cmd)
            {
                cmd.Parameters.Clear();
                using (var rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        drivers.Add(new Drivers
                        {
                            DriverId = rdr.GetString("DriverId"),
                            LastName = rdr.GetString("LastName"),
                            FirstName = rdr.GetString("FirstName"),
                            MiddleInitial = rdr.GetString("MiddleInitial"),
                            Extension = rdr.IsDBNull("NameExtension") ? string.Empty : rdr.GetString("NameExtension"),
                            Address = rdr.GetString("address"),
                            Phone = rdr.GetString("phone")
                        });
                    }
                }
            }
            DriverList.Clear();
            foreach (var d in drivers)
            {
                DriverList.Add(d);
            }
        }


        public void CountDrivers()
        {
            server.Connect();
            cmd.Connection = server.connection;
            cmd.CommandText = @"SELECT COUNT(*) AS dcount FROM drivers;";
            using (cmd)
            {
                cmd.Parameters.Clear();
                using (var rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        DCount = rdr.GetInt32("dcount");
                    }
                    else
                    {
                        DCount = 0;
                    }
                }

            }
        }

        public void DeleteDriver(String driverid)
        {
            server.Connect();
            cmd.Connection = server.connection;
            cmd.CommandText = @"DELETE FROM drivers WHERE DriverId=@driverid";
            using (cmd)
            {
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@driverid", driverid);
                cmd.ExecuteNonQuery();
            }
        }

        //public int NewDriver(Drivers driver)
        //{
        //    string IsExistingId = @"SELECT DriverId FROM drivers WHERE DriverId=@driverid";
        //    using (var conn = new MySqlConnection(server.server))
        //    {
        //        conn.Open();
        //        using (var cmd = new MySqlCommand(IsExistingId, conn))
        //        {
        //            cmd.Parameters.Clear();
        //            cmd.Parameters.AddWithValue("@driverid", driver.DriverId);
        //            using (var rdr = cmd.ExecuteReader())
        //            {
        //                if (rdr.Read())
        //                {
        //                    return 0;
        //                } else
        //                {
        //                    using (var cmd2 = new MySqlCommand(@"INSERT INTO drivers (DriverId, LastName, FirstName, MiddleInitial, NameExtension, address, phone)
        //                VALUES (@driverid, @lname, @fname, @mi, @ex, @address, @phone);", conn))
        //                    {
        //                        cmd2.Parameters.Clear();
        //                        cmd2.Parameters.AddWithValue("@driverid", driver.DriverId);
        //                        cmd2.Parameters.AddWithValue("@lname", driver.LastName);
        //                        cmd2.Parameters.AddWithValue("@fname", driver.FirstName);
        //                        cmd2.Parameters.AddWithValue("@mi", driver.MiddleInitial);
        //                        cmd2.Parameters.AddWithValue("@ex", driver.Extension);
        //                        cmd2.Parameters.AddWithValue("@address", driver.Address);
        //                        cmd2.Parameters.AddWithValue("@phone", driver.Phone);
        //                        cmd2.ExecuteNonQuery();
        //                    }

        //                    using (var addnew = new MySqlCommand(@"INSERT INTO tricycles (DriverId, TricycleNo, capacity)
        //                                   VALUES (@driverid, @tno, @capacity)", conn))
        //                    {
        //                        addnew.Parameters.Clear();
        //                        addnew.Parameters.AddWithValue("@driverid", driver.DriverId);
        //                        addnew.Parameters.AddWithValue("@tno", driver.TricycleNo);
        //                        addnew.Parameters.AddWithValue("@capacity", driver.Capacity);
        //                        addnew.ExecuteNonQuery();
        //                    }

        //                    return 1;
        //                }
        //            }
        //        }
        //    }
        public int NewDriver(Drivers driver)
        {
            string IsExistingId = @"SELECT DriverId FROM drivers WHERE DriverId=@driverid";
            using (var conn = new MySqlConnection(server.server))
            {
                conn.Open();
                bool exists = false;

                // First check if the driver already exists
                using (var cmd = new MySqlCommand(IsExistingId, conn))
                {
                    cmd.Parameters.AddWithValue("@driverid", driver.DriverId);
                    using (var rdr = cmd.ExecuteReader())
                    {
                        if (rdr.Read())
                        {
                            exists = true;
                        }
                    } // Reader is closed here
                }

                if (exists)
                {
                    return 0;
                }
                else
                {
                    // Insert into drivers
                    using (var cmd2 = new MySqlCommand(@"INSERT INTO drivers 
                (DriverId, LastName, FirstName, MiddleInitial, NameExtension, address, phone)
                VALUES (@driverid, @lname, @fname, @mi, @ex, @address, @phone);", conn))
                    {
                        cmd2.Parameters.AddWithValue("@driverid", driver.DriverId);
                        cmd2.Parameters.AddWithValue("@lname", driver.LastName);
                        cmd2.Parameters.AddWithValue("@fname", driver.FirstName);
                        cmd2.Parameters.AddWithValue("@mi", driver.MiddleInitial);
                        cmd2.Parameters.AddWithValue("@ex", driver.Extension);
                        cmd2.Parameters.AddWithValue("@address", driver.Address);
                        cmd2.Parameters.AddWithValue("@phone", driver.Phone);
                        cmd2.ExecuteNonQuery();
                    }

                    // Insert into tricycles
                    using (var addnew = new MySqlCommand(@"INSERT INTO tricycles 
                (DriverId, TricycleNo, capacity)
                VALUES (@driverid, @tno, @capacity)", conn))
                    {
                        addnew.Parameters.AddWithValue("@driverid", driver.DriverId);
                        addnew.Parameters.AddWithValue("@tno", driver.TricycleNo);
                        addnew.Parameters.AddWithValue("@capacity", driver.Capacity);
                        addnew.ExecuteNonQuery();
                    }

                    return 1;
                }
            }
        }


        public void CountAvailable()
        {
            server.Connect();
            cmd.Connection = server.connection;
            cmd.CommandText = @"SELECT COUNT(*) AS availabled FROM activedrivers WHERE status='available'";
            using (cmd)
            {
                cmd.Parameters.Clear();
                using (var rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        AvailableDriver = rdr.GetInt32("availabled");
                    } else
                    {
                        AvailableDriver = 0;
                    }
                }
            }
        }

        public void CountBooked()
        {
            server.Connect();
            cmd.Connection = server.connection;
            cmd.CommandText = @"SELECT COUNT(*) AS booked FROM activedrivers WHERE status='booked'";
            using (cmd)
            {
                cmd.Parameters.Clear();
                using (var rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        BookedDriver = rdr.GetInt32("booked");
                    }
                    else
                    {
                        BookedDriver = 0;
                    }
                }
            }
        }

        public void CountPending()
        {
            server.Connect();
            cmd.Connection = server.connection;
            cmd.CommandText = @"SELECT COUNT(*) AS pendingb FROM bookingtransactions WHERE status='pending' AND DATE(BookingDt)=CURDATE()";
            using (cmd)
            {
                cmd.Parameters.Clear();
                using (var rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        Pending = rdr.GetInt32("pendingb");
                    }
                    else
                    {
                        Pending = 0;
                    }
                }
            }
        }

        public void CountCanceled()
        {
            server.Connect();
            cmd.Connection = server.connection;
            cmd.CommandText = @"SELECT COUNT(*) AS canceledb FROM bookingtransactions WHERE status='canceled' AND DATE(BookingDt)=CURDATE()";
            using (cmd)
            {
                cmd.Parameters.Clear();
                using (var rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        Canceled = rdr.GetInt32("canceledb");
                    }
                    else
                    {
                        Canceled = 0;
                    }
                }
            }
        }

        public void CountCompleted()
        {
            server.Connect();
            cmd.Connection = server.connection;
            cmd.CommandText = @"SELECT COUNT(*) AS completedb FROM bookingtransactions WHERE status='completed' AND DATE(BookingDt)=CURDATE()";
            using (cmd)
            {
                cmd.Parameters.Clear();
                using (var rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        Completed = rdr.GetInt32("completedb");
                    }
                    else
                    {
                        Completed = 0;
                    }
                }
            }
        }

        public void CountOngoing()
        {
            server.Connect();
            cmd.Connection = server.connection;
            cmd.CommandText = @"SELECT COUNT(*) AS ongoingb FROM bookingtransactions WHERE status='ongoing' AND DATE(BookingDt)=CURDATE()";
            using (cmd)
            {
                cmd.Parameters.Clear();
                using (var rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        Ongoing = rdr.GetInt32("ongoingb");
                    }
                    else
                    {
                        Ongoing = 0;
                    }
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
