using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using MySql.Data.MySqlClient;
using PMA.Modules;

namespace PMA.Passenger
{
    public class UserService
    {
        Server server = new Server();
        MySqlCommand cmd = new MySqlCommand();


        // service for getting user information upon initialization
        public Users GetUserById(string userId)
        {
            var user = new Users();

            server.Connect();
            cmd.Connection = server.connection;
            cmd.CommandText = @"SELECT UserId, FirstName, LastName, MiddleInitial, NameExtension, email, phone 
                                FROM users WHERE UserId = @UserId;";
            using (cmd)
            {
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@UserId", userId);

                using (var rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        user.UserId = rdr.GetString("UserId");
                        user.FirstName = rdr.GetString("FirstName").ToUpper();
                        user.LastName = rdr.GetString("LastName").ToUpper();
                        user.MiddleInitial = rdr.GetString("MiddleInitial").ToUpper();

                        var nameExtensionObj = rdr["NameExtension"];
                        user.Extension = nameExtensionObj == DBNull.Value ? "" : nameExtensionObj.ToString().ToUpper();

                        user.Email = rdr.GetString("email");
                        user.Phone = rdr.GetString("phone");
                        user.FullName = $"{user.LastName}, {user.FirstName} {user.MiddleInitial} {user.Extension}";
                    }
                }
            }
            return user;
        }

        public Book LoadAvailableDriver()
        {
            var book = new Book();
            server.Connect();
            cmd.Connection = server.connection;
            cmd.CommandText = @"SELECT activedrivers.DriverId, drivers.FirstName, drivers.LastName, drivers.MiddleInitial, drivers.NameExtension, tricycles.capacity, (tricycles.capacity - activedrivers.assignedseats) AS AvailableSeats
                                                FROM activedrivers 
                                                JOIN drivers ON activedrivers.DriverId=drivers.DriverId 
                                                JOIN tricycles ON activedrivers.TricycleId=tricycles.id
                                                WHERE status='available'
                                                ORDER BY trips DESC;";

            using (cmd)
            {
                cmd.Parameters.Clear();
                using (var rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        string nameExtension = rdr["NameExtension"] == DBNull.Value ? "" : rdr["NameExtension"].ToString().ToUpper();

                        book.DriverName = rdr["LastName"].ToString().ToUpper() + ", " +
                                          rdr["FirstName"].ToString().ToUpper() + " " +
                                          rdr["MiddleInitial"].ToString().ToUpper() + ". " +
                                          nameExtension;
                        book.DriverId = rdr.GetString("DriverId");
                        book.AvailableSeats = rdr.GetInt16("AvailableSeats");
                    }
                    else
                    {
                        book.DriverName = "Waiting for available driver...";
                    }
                }
            }

            return book;
        }

        public void SendBooking(Book booking, Users user) 
        {
            server.Connect();
            cmd.Connection = server.connection;
            cmd.CommandText = @"INSERT INTO bookingtransactions (DriverId, UserId, location, seats)
                                                VALUES (@BookedDriverId, @PassengerId, @location, @seats);";
            using (cmd)
            {
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@BookedDriverId", booking.DriverId);
                cmd.Parameters.AddWithValue("@PassengerId", user.UserId);
                cmd.Parameters.AddWithValue("@location", booking.Location);
                cmd.Parameters.AddWithValue("@seats", booking.BookedSeats);
                cmd.ExecuteNonQuery();

                cmd.CommandText = @"UPDATE activedrivers
                            SET assignedseats = assignedseats + @bookedSeats
                            WHERE DriverId = @BookedDriverId;";
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@bookedSeats", booking.BookedSeats);
                cmd.Parameters.AddWithValue("@BookedDriverId", booking.DriverId);
                cmd.ExecuteNonQuery();
            }
        }

        public Book GetBookingInfo(Book book, Users user)
        {
            var info = new Book();
            server.Connect();
            cmd.Connection = server.connection;
            cmd.CommandText = @"SELECT users.FirstName AS ufname, users.LastName AS ulname, users.MiddleInitial AS umi, users.NameExtension AS uex,
                                            drivers.FirstName AS dfname, drivers.LastName AS dlname, drivers.MiddleInitial AS dmi, drivers.NameExtension AS dex,
                                            bookingtransactions.id AS bid, bookingtransactions.location AS location, bookingtransactions.seats AS seats, bookingtransactions.BookingDT, bookingtransactions.status AS status
                                            FROM bookingtransactions
                                            JOIN users ON bookingtransactions.UserId=users.UserId
                                            JOIN drivers ON bookingtransactions.DriverId=drivers.DriverId
                                            WHERE bookingtransactions.UserId=@UserId AND status NOT IN ('canceled', 'completed') 
                                            AND DATE(bookingtransactions.BookingDT)=CURDATE()
                                            ORDER BY bid DESC
                                            LIMIT 1;";
            using (cmd)
            {
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@UserId", user.UserId);

                using (var rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        string passengerNameExtension = rdr.IsDBNull(rdr.GetOrdinal("uex")) ? "" : rdr.GetString("uex");
                        string driverNameExtension = rdr.IsDBNull(rdr.GetOrdinal("dex")) ? "" : rdr.GetString("dex");

                        info.PassengerName = $"{rdr.GetString("ulname").ToUpper()}, {rdr.GetString("ufname").ToUpper()} {rdr.GetString("umi").ToUpper()} {passengerNameExtension.ToUpper()}";
                        info.DriverName = $"{rdr.GetString("dlname").ToUpper()}, {rdr.GetString("dfname").ToUpper()} {rdr.GetString("dmi").ToUpper()} {driverNameExtension.ToUpper()}";

                        info.Location = rdr.GetString("location");
                        info.BookedSeats = rdr.GetInt32("seats");
                        info.Status = rdr.GetString("status");
                        info.BookingId = rdr.GetInt32("bid");
                        info.BookingDT = rdr.GetDateTime(rdr.GetOrdinal("BookingDT")); // ← ADD THIS
                    }
                }
            }
            return info.BookingId > 0 ? info : null;
        }


        public string GetGoogleMap(string location)
        {
            return $"https://www.google.com/maps?q={Uri.EscapeDataString(location)}";
        }

        public void OnCancellation(int bookingid)
        {
            server.Connect();
            cmd.Connection = server.connection;
            cmd.CommandText = @"UPDATE bookingtransactions SET status=@status WHERE id=@id;";
            using (cmd)
            {
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@status", "canceled");
                cmd.Parameters.AddWithValue("@id", bookingid);
                cmd.ExecuteNonQuery();
            }
        }

        public void OnCompletion(int bookingid)
        {
            server.Connect();
            cmd.Connection = server.connection;
            cmd.CommandText = @"UPDATE bookingtransactions SET status=@status WHERE id=@id;";
            using (cmd)
            {
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@status", "completed");
                cmd.Parameters.AddWithValue("@id", bookingid);
                cmd.ExecuteNonQuery();
            }
        }

        public String CheckingStatus(int BookingId)
        {
            String UpdatedStat = "";
            server.Connect();
            cmd.Connection = server.connection;
            cmd.CommandText = @"SELECT status FROM bookingtransactions
                                                    WHERE id=@BookingId;";
            using (cmd)
            {
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@BookingId", BookingId);
                using (var rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        UpdatedStat = rdr.GetString("status");
                    }
                }
            }
            return UpdatedStat;
        }

        public List<Book> PastBooking(string UserId)
        {
            var bookings = new List<Book>();
            server.Connect();
            cmd.Connection = server.connection;
            cmd.CommandText = @"
        SELECT drivers.FirstName AS dfname, drivers.LastName AS dlname, drivers.MiddleInitial AS dmi, drivers.NameExtension AS dex,
               bookingtransactions.location AS location, bookingtransactions.seats AS seats, bookingtransactions.BookingDT AS bdt, 
               bookingtransactions.status AS status
        FROM bookingtransactions
        JOIN drivers ON bookingtransactions.DriverId = drivers.DriverId
        WHERE bookingtransactions.UserId = @UserId AND status IN ('canceled', 'completed')
        ORDER BY bookingtransactions.BookingDT DESC;";

            using (cmd)
            {
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@UserId", UserId);

                using (var rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        string driverExt = rdr.IsDBNull(rdr.GetOrdinal("dex")) ? "" : rdr.GetString("dex");
                        string firstName = rdr.GetString(rdr.GetOrdinal("dfname"));
                        string lastName = rdr.GetString(rdr.GetOrdinal("dlname"));
                        string location = rdr.IsDBNull(rdr.GetOrdinal("location")) ? "Unknown" : rdr.GetString(rdr.GetOrdinal("location"));
                        int seats = rdr.IsDBNull(rdr.GetOrdinal("seats")) ? 0 : rdr.GetInt32(rdr.GetOrdinal("seats"));
                        string status = rdr.IsDBNull(rdr.GetOrdinal("status")) ? "Unknown" : rdr.GetString(rdr.GetOrdinal("status"));
                        DateTime bookingDT = rdr.GetDateTime(rdr.GetOrdinal("bdt"));

                        bookings.Add(new Book
                        {
                            BookedDriver = $"{lastName.ToUpper()}, {firstName.ToUpper()} {driverExt.ToUpper()}",
                            Location = location,
                            BookedSeats = seats,
                            BookingDT = bookingDT,
                            Status = status
                        });
                    }
                }
            }

            return bookings;
        }


        public void UpdateUserProfile(Users user)
        {
            server.Connect();
            cmd.Connection = server.connection;
            cmd.CommandText = @"UPDATE users 
                                            SET FirstName=@fname, LastName=@lname, MiddleInitial=@mi, NameExtension=@ex, email=@email, phone=@phone
                                            WHERE UserId=@userid;";

            using (cmd)
            {
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@fname", user.FirstName);
                cmd.Parameters.AddWithValue("@lname", user.LastName);
                cmd.Parameters.AddWithValue("@mi", user.MiddleInitial);
                cmd.Parameters.AddWithValue("@ex", user.Extension);
                cmd.Parameters.AddWithValue("@email", user.Email);
                cmd.Parameters.AddWithValue("@phone", user.Phone);
                cmd.Parameters.AddWithValue("@userid", user.UserId);
                cmd.ExecuteNonQuery();
            }
        }

        public void ChangePassword(Users user)
        {
            String HashPassword = BCrypt.Net.BCrypt.EnhancedHashPassword(user.Password);
            server.Connect();
            cmd.Connection = server.connection;
            cmd.CommandText = @"UPDATE users SET password=@newpassword WHERE UserId=@userid;";
            using(cmd)
            {
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@newpassword", HashPassword);
                cmd.Parameters.AddWithValue("@userid", user.UserId);
                cmd.ExecuteNonQuery();
            }
        }
    }
}
