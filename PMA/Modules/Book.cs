using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMA.Modules
{
    public class Book
    {
        public string DriverId { get; set; }
        public string DriverName { get; set; }

        public string Location { get; set; }
        public int BookedSeats { get; set; }
        public int AvailableSeats { get; set; }

        public int BookingId { get; set; }
        public string Status { get; set; }

        public string PassengerName { get; set; }
        public DateTime BookingDT { get; set; }

        public String BookedDriver {  get; set; }
    }
}
