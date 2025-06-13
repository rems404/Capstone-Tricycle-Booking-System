using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using PMA.Modules;

namespace PMA.Passenger
{
    public class UserViewModel : INotifyPropertyChanged
    {
        Server server = new Server();

        private readonly UserService _userservice = new UserService();

        private string _userid;
        private string _status;
        private string _fname;
        private string _lname;
        private string _mi;
        private string _ex;
        private string _email;
        private string _phone;
        public string _password;
        public string _fullname;
        public string _driverid;
        public string _drivername;
        public string _location;
        public int _bookedseats;
        public int _availableseats;
        private int _bookingid;
        private DateTime _bookingdt;

        private double _latitude;
        private double _longitude;


        public double Latitude
        {
            get => _latitude;
            set { _latitude = value; OnPropertyChanged(); }
        }

        public double Longitude
        {
            get => _longitude;
            set { _longitude = value; OnPropertyChanged(); }
        }


        public string UserId
        {
            get => _userid;
            set
            {
                _userid = value;
                OnPropertyChanged(nameof(UserId));
            }
        }

        public string FirstName
        {
            get => _fname;
            set
            {
                _fname = value;
                OnPropertyChanged(nameof(FirstName));
            }
        }

        public string LastName
        {
            get => _lname;
            set
            {
                _lname = value;
                OnPropertyChanged(nameof(LastName));
            }
        }

        public string MiddleInitial
        {
            get => _mi;
            set
            {
                _mi = value;
                OnPropertyChanged(nameof(MiddleInitial));
            }
        }

        public string Extension
        {
            get => _ex;
            set
            {
                _ex = value;
                OnPropertyChanged(nameof(Extension));
            }
        }

        public string Email
        {
            get => _email;
            set
            {
                _email = value;
                OnPropertyChanged(nameof(Email));
            }
        }

        public string Phone
        {
            get => _phone;
            set
            {
                _phone = value;
                OnPropertyChanged(nameof(Phone));
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged(nameof(Password));
            }
        }

        public string FullName
        {
            get => _fullname;
            set
            {
                _fullname = value;
                OnPropertyChanged(nameof(FullName));
            }
        }

        public string DriverId
        {
            get => _driverid;
            set
            {
                _driverid = value;
                OnPropertyChanged(nameof(DriverId));
            }
        }

        public string DriverName
        {
            get => _drivername;
            set
            {
                _drivername = value;
                OnPropertyChanged(nameof(DriverName));
            }
        }

        public int AvailableSeats
        {
            get => _availableseats;
            set
            {
                _availableseats = value;
                OnPropertyChanged(nameof(AvailableSeats));
            }
        }

        public string Location
        {
            get => _location;
            set
            {
                _location = value;
                OnPropertyChanged(nameof(Location));
            }
        }

        public int BookedSeats
        {
            get => _bookedseats;
            set
            {
                _bookedseats = value;
                OnPropertyChanged(nameof(BookedSeats));
            }
        }

        public int BookingId
        {
            get => _bookingid;
            set
            {
                _bookingid = value;
                OnPropertyChanged(nameof(BookingId));
            }
        }

        public string Status
        {
            get => _status;
            set
            {
                if (_status != value)
                {
                    _status = value;
                    OnPropertyChanged(nameof(Status));
                }
            }
        }

        public DateTime BookingDT
        {
            get => _bookingdt;
            set
            {
                if (_bookingdt != value)
                {
                    _bookingdt = value;
                    OnPropertyChanged(nameof(BookingDT));
                }
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public async Task LoadUser(String id)
        {
            var user = _userservice.GetUserById(id);
            UserId = user.UserId;
            FirstName = user.FirstName;
            LastName = user.LastName;
            MiddleInitial = user.MiddleInitial;
            Extension = user.Extension;
            Email = user.Email;
            Phone = user.Phone;
            FullName = user.FullName;
        }

        public void CancelBooking()
        {
            _userservice.OnCancellation(BookingId);
        }

        public void CompletedBooking()
        {
            _userservice.OnCompletion(BookingId);
        }

        public void BookingStatusChecker()
        {
            var stat = _userservice.CheckingStatus(BookingId);
            if (!string.IsNullOrWhiteSpace(stat) && stat != Status)
            {
                Status = stat;
            }
        }

        public async Task LoadDriverInfo()
        {
            var book = _userservice.LoadAvailableDriver();
            DriverId = book.DriverId;
            DriverName = book.DriverName;
            AvailableSeats = book.AvailableSeats;
        }


        // for sending a new booking
        public void SendNewBooking()
        {
            var user = new Users
            {
                UserId = this._userid
            };

            var book = new Book
            {
                DriverId = this._driverid,
                DriverName = this._drivername,
                Location = this._location,
                BookedSeats = this._bookedseats
            };
            _userservice.SendBooking(book, user);
        }

        public void ProfileUpdate()
        {
            var updatedUser = new Users
            {
                UserId = this.UserId,
                FirstName = this.FirstName,
                LastName = this.LastName,
                MiddleInitial = this.MiddleInitial,
                Extension = this.Extension,
                Email = this.Email,
                Phone = this.Phone
            };

            _userservice.UpdateUserProfile(updatedUser);
            LoadUser(UserId);
        }

        public void PasswordUpdate()
        {
            var UpdatedPsw = new Users
            {
                UserId = this.UserId,
                Password = this.Password
            };
            _userservice.ChangePassword(UpdatedPsw);
        }


        public async Task LoadBookingInfo()
        {
            var user = new Users { UserId = this._userid };
            var book = new Book { DriverId = this._driverid };
            var info = _userservice.GetBookingInfo(book, user);
            if (info != null)
            {
                FullName = info.PassengerName;
                DriverName = info.DriverName;
                Location = info.Location;
                BookedSeats = info.BookedSeats;
                Status = info.Status;
                BookingId = info.BookingId;
                BookingDT = info.BookingDT;
            }
            else
            {
                BookingId = 0;
            }
        }

        // for map display and location tracker
        public async Task<string> GetLocationHtml()
        {
            try
            {
                var request = new GeolocationRequest(GeolocationAccuracy.Medium);
                var location = await Geolocation.GetLocationAsync(request);

                if (location != null)
                {
                    var latitude = location.Latitude;
                    var longitude = location.Longitude;

                    return $@"
                <html>
                <body style='margin:0;padding:0;'>
                    <iframe
                        width='100%' height='100%'
                        frameborder='0' style='border:0'
                        src='https://maps.google.com/maps?q={latitude},{longitude}&z=15&output=embed' allowfullscreen>
                    </iframe>
                </body>
                </html>";
                } 
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Geolocation error: {ex.Message}");
            }

            return "<html><body><p>Location unavailable.</p></body></html>";
        }
    }
}
