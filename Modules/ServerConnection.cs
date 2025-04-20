using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace Passenger.Modules
{
    public class ServerConnection
    {
        public MySqlConnection conn = new MySqlConnection();
        public String conn_string = "server=localhost; uid=root; psw=; database=pickmeapp";

        
        public void connect()
        {
            if (conn.State != System.Data.ConnectionState.Open)
            {
                conn.ConnectionString = conn_string; 
                conn.Open(); 
            }
        }
    }
}
