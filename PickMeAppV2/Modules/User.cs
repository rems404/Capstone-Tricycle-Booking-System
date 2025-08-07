using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Org.BouncyCastle.Bcpg;

namespace PickMeAppV2.Modules
{
    public class User
    {
        public String UserId { get; set; }

        // personal information
        public String FirstName {  get; set; }
        public String LastName { get; set; }
        public String MiddleInitial {  get; set; }
        public String Extension { get; set; }
        public String Email { get; set; }
        public String Phone { get; set; }
        public String Password { get; set; }

        public DateTime JoinedAt { get; set; }

        public String FullName {  get; set; }
    }
}
