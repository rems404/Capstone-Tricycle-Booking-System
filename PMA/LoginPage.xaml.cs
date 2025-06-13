using MySql.Data.MySqlClient;
using BCrypt.Net;
using PMA.Modules;
using PMA.Admin;
using PMA.Passenger;

namespace PMA;

public partial class LoginPage : ContentPage
{
    Users user = new Users();
    Server server = new Server();

    public LoginPage()
    {
        InitializeComponent();

        UserIdTbx.TextChanged += (sender, e) => Validate();
        PasswordTbx.TextChanged += (sender, e) => Validate();
    }

    //---------- methods ----------//
    private void Validate()
    {
        if (!string.IsNullOrWhiteSpace(UserIdTbx.Text))
        {
            user.UserId = UserIdTbx.Text;
        }

        if (!string.IsNullOrWhiteSpace(PasswordTbx.Text))
        {
            user.Password = PasswordTbx.Text;
        }

        LoginBtn.IsEnabled = !string.IsNullOrWhiteSpace(UserIdTbx.Text) && !string.IsNullOrWhiteSpace(PasswordTbx.Text);
    }

    //---------- event handlers ----------//
    private void LoginBtn_Clicked(object sender, EventArgs e)
    {
        CheckIfUser(user.UserId, user.Password);
    }

    private async void GoToSignUpBtn_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new RegistrationPage());
    }

    //---------- queries ----------//
    private async void CheckIfUser(String userid, String password)
    {
        String query = @"SELECT password, role FROM users WHERE UserId=@userid";

        try
        {
            using (var conn = new MySqlConnection(server.server))
            {
                conn.Open();
                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@userid", userid);
                    using (var rdr = cmd.ExecuteReader())
                    {
                        if (!rdr.HasRows)
                        {
                            GlobalError.Text = "User not found";
                        }

                        while (rdr.Read())
                        {
                            String StoredPsw = rdr.GetString("password");
                            String role = rdr.GetString("role");
                            bool IsCorrect = BCrypt.Net.BCrypt.EnhancedVerify(password, StoredPsw);

                            if (IsCorrect)
                            {
                                switch (role)
                                {
                                    case "user":
                                        await Navigation.PushAsync(new UserLanding(userid));
                                        UserIdTbx.Text = "";
                                        PasswordTbx.Text = "";
                                        break;
                                    case "admin":
                                        await Navigation.PushAsync(new AdminLanding(userid));
                                        UserIdTbx.Text = "";
                                        PasswordTbx.Text = "";
                                        break;
                                }
                            }
                            else
                            {
                                GlobalError.Text = "Incorrect ID or password...";
                            }
                        }
                    }
                }
            }
        } catch (Exception ex)
        {
            GlobalError.Text = "ERROR: " + ex.Message;
        }
    }
}