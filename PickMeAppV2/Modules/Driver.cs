using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PickMeAppV2.Modules
{
    public class Driver
    {
        public String DriverId { get; set; }
        public String FirstName { get; set; }
        public String LastName { get; set; }
        public String MiddleInitial { get; set; }
        public String Extension { get; set; }
        public String Address { get; set; }
        public String Phone { get; set; }
        public String TricycleNo { get; set; }
        public int Capacity { get; set; }
        public String FullName { get; set; }
        public int AvailableSeats { get; set; }
        public String Status { get; set; }
        public int Trips { get; set; }
    }
}
