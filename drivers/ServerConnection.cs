using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace PickMeApp_Admin.Module
{
    public  class ServerConnection
    {
        public MySqlConnection conn = new MySqlConnection();
        public String admin_conn_string = "server=localhost; uid=root; psw=; database=pickmeapp_admin";

        public void ConnectAdmin()
        {
            if(conn.State != System.Data.ConnectionState.Open)
            {
                conn.ConnectionString = admin_conn_string;
                conn.Open();
            }
        }
    }
}
