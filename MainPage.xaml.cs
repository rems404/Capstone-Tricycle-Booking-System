// Members: Althea Carrera, Mary Grace Dutdut, Jasmin Kate Los Baños, Remy Dianne Ventura
using MySql.Data.MySqlClient;

namespace pickMeAppPass
{
    public partial class MainPage : ContentPage
    {
        MySqlConnection conn = new MySqlConnection();
        String conn_string = "server=localhost; uid=root; password=; database=pickMeApp";
        MySqlDataReader rdr;

        public void connect()
        {
            conn.ConnectionString = conn_string;
            conn.Open();
        }

        public MainPage()
        {
            InitializeComponent();
        }

        public void store_data()
        {
            connect();
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = "INSERT INTO users (studId, firstName, lastName, email, phoneNo, username, password) VALUES (@studId, @firstName, @lastName, @email, @phoneNo, @username, @password)";
            using (conn)
            {
                cmd.Parameters.AddWithValue("@studId", this.studId_tbx.Text);
                cmd.Parameters.AddWithValue("@firstName", this.fname_tbx.Text);
                cmd.Parameters.AddWithValue("@lastName", this.lname_tbx.Text);
                cmd.Parameters.AddWithValue("@email", this.email_tbx.Text);
                cmd.Parameters.AddWithValue("@phoneNo", this.phoneNo_tbx.Text);
                cmd.Parameters.AddWithValue("@username", this.username_tbx.Text);
                cmd.Parameters.AddWithValue("@password", this.password_tbx.Text);
                cmd.ExecuteNonQuery();
            }
        }

        public void signUp_clicked (object sender, EventArgs e)
        {
            store_data();
        }
    }

}
