using BCrypt.Net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Passenger.Modules
{
    public class EnDecrypt
    {
        public String hashPassword(String password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }
    }
}
