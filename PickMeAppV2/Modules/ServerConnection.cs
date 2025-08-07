using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace PickMeAppV2.Modules
{
    public class ServerConnection
    {
        private String ConnectionString = "server=localhost; user=root; password=; database=pickmeapp";
        //private String ConnectionString = "server=192.168.8.39; user=pma_user; password=pm@4@2526; database=pickmeapp";

        public MySqlConnection GetConnection()
        {
            return new MySqlConnection(ConnectionString);
        }
    }
}
