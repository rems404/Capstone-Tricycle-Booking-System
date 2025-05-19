using MySql.Data.MySqlClient;
using PickMeApp_Admin.Module;
using BCrypt.Net;

namespace PickMeApp_Admin;

    public partial class MainPage : ContentPage
    {
        public ServerConnection cnnt = new ServerConnection();
        public MySqlDataReader rdr;
        //public String username;
        //public String password; 
        public MainPage()
        {
            InitializeComponent();
            username_tbx.TextChanged += (sender, e) => ValidateField();
            password_tbx.TextChanged += (sender, e) => ValidateField();
        }

        public void ValidateField()
        {
            //username = username_tbx.Text;
            //password = password_tbx.Text;

            login_btn.IsEnabled = !string.IsNullOrEmpty(username_tbx.Text) && !string.IsNullOrEmpty(password_tbx.Text);
        }

        private async void isAdmin(string username, string enteredPassword)
        {
            cnnt.ConnectAdmin();

            using (var cmd = new MySqlCommand("SELECT password FROM admin WHERE username=@username", cnnt.conn))
            {
                cmd.Parameters.AddWithValue("@username", username);

                using (var rdr = cmd.ExecuteReader())
                {
                    if (!rdr.HasRows)
                    {
                        await DisplayAlert("ERROR", "Admin not found", "OK");
                        return;
                    }
                    while (rdr.Read())
                    {
                        string storedPassword = rdr.GetString(0).Trim();

                        if (enteredPassword == storedPassword)
                        //if (BCrypt.Net.BCrypt.Verify(enteredPassword, storedPassword))
                        {
                            await DisplayAlert("SUCCESS", "Welcome, Admin!", "OK");
                        await Navigation.PushAsync(new NewPage1(username));
                        }
                        else
                        {
                            await DisplayAlert("INCORRECT", "Wrong Password", "OK");
                        }
                    }
                }
            }
        }
        async void LoginClicked(object sender, EventArgs e)
        {
            isAdmin(username_tbx.Text, password_tbx.Text);
        }
    }
