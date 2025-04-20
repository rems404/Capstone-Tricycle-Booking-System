using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passenger.Modules
{
    public class User
    {
        public String studId { get; set; } = "";
        public String lastName { get; set; } = "";
        public String firstName { get; set; } = "";
        public String email { get; set; } = "";
        public String phone { get; set; } = "";
        public String username { get; set; } = "";
        public String password { get; set; } = "";

        public User(String studId, String lastName, String firstName, String email, String phone)
        {
            this.studId = studId;
            this.lastName = lastName;
            this.firstName = firstName;
            this.email = email;
            this.phone = phone;
        }

        public User(String username, String password)
        {
            this.username = username;
            this.password = password;
        }
    }
}
