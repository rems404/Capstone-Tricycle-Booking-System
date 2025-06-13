using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace PMA.Modules
{
    public class Server
    {
        public MySqlConnection connection = new MySqlConnection();
        public String server = "server=localhost; uid=root; password=; database=pma";

        public void Connect()
        {
            if (connection == null || connection.State == System.Data.ConnectionState.Closed || connection.State == System.Data.ConnectionState.Broken)
            {
                connection = new MySqlConnection(server);
                connection.Open();
            }
        }
    }
}
