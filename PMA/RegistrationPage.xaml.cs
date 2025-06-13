using MySql.Data.MySqlClient;
using PMA.Modules;

namespace PMA;

public partial class RegistrationPage : ContentPage
{
    Server server = new Server();
    Users user = new Users();

    public RegistrationPage()
    {
        InitializeComponent();
        UserIdTbx.TextChanged += (sender, e) => Validate();
        FnameTbx.TextChanged += (sender, e) => Validate();
        LnameTbx.TextChanged += (sender, e) => Validate();
        MITbx.TextChanged += (sender, e) => Validate();
        ExtensionTbx.TextChanged += (sender, e) => Validate();
        EmailTbx.TextChanged += (sender, e) => Validate();
        PhoneTbx.TextChanged += (sender, e) => Validate();
        PasswordTbx.TextChanged += (sender, e) => Validate();
        ConfirmationTbx.TextChanged += (sender, e) => Validate();
    }

    private void Validate()
    {
        if (string.IsNullOrWhiteSpace(UserIdTbx.Text))
        {
            UserIdTbx.Placeholder = "User ID cannot be empty.";
        }
        else
        {
            user.UserId = UserIdTbx.Text;
        }

        if (string.IsNullOrWhiteSpace(FnameTbx.Text))
        {
            FnameTbx.Placeholder = "First name cannot be empty.";
        }
        else
        {
            user.FirstName = FnameTbx.Text;
        }

        if (string.IsNullOrWhiteSpace(LnameTbx.Text))
        {
            LnameTbx.Placeholder = "Last name cannot be empty.";
        }
        else
        {
            user.LastName = LnameTbx.Text;
        }

        if (string.IsNullOrWhiteSpace(MITbx.Text))
        {
            MITbx.Placeholder = "Middle initial cannot be empty.";
        }
        else
        {
            user.MiddleInitial = MITbx.Text;
        }

        if (!string.IsNullOrWhiteSpace(ExtensionTbx.Text))
        {
            user.Extension = ExtensionTbx.Text;
        }

        if (string.IsNullOrWhiteSpace(EmailTbx.Text))
        {
            EmailTbx.Placeholder = "Email cannot be empty.";
        }
        else
        {
            user.Email = EmailTbx.Text;
        }

        if (string.IsNullOrWhiteSpace(PhoneTbx.Text))
        {
            PhoneTbx.Placeholder = "Phone number cannot be empty.";
        }
        else
        {
            user.Phone = PhoneTbx.Text;
        }

        if (string.IsNullOrWhiteSpace(PasswordTbx.Text))
        {
            PasswordTbx.Placeholder = "Please set up password.";
        }

        if (string.IsNullOrWhiteSpace(ConfirmationTbx.Text))
        {
            ConfirmationTbx.Placeholder = "Confirm password...";
        }

        if (PasswordTbx.Text != ConfirmationTbx.Text)
        {
            GlobalError.Text = "Passwords do not match.";
        }
        else
        {
            user.Password = PasswordTbx.Text;
        }

        if (user.UserId == null || user.FirstName == null || user.LastName == null ||
            user.MiddleInitial == null || user.Email == null || user.Phone == null || user.Password == null)
        {
            SignUpBtn.IsEnabled = false;
        }
        else
        {
            SignUpBtn.IsEnabled = true;
        }
    }

    private async void SendRequest(String _UserId, String _Fname, String _Lname, String _MI, String _ex, String _email, String _phone, String _password)
    {
        String HashPsw = BCrypt.Net.BCrypt.EnhancedHashPassword(_password);

        server.Connect();
        try
        {
            using (var cmd = new MySqlCommand(@"INSERT INTO users (UserId, FirstName, LastName, MiddleInitial, NameExtension, email, phone, password, role)
                                                VALUES (@UserId, @FirstName, @LastName, @MiddleInitial, @NameExtension, @email, @phone, @password, @role)", server.connection))
            {
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@UserId", _UserId);
                cmd.Parameters.AddWithValue("@FirstName", _Fname);
                cmd.Parameters.AddWithValue("@LastName", _Lname);
                cmd.Parameters.AddWithValue("@MiddleInitial", _MI);
                cmd.Parameters.AddWithValue("@NameExtension", _ex);
                cmd.Parameters.AddWithValue("@email", _email);
                cmd.Parameters.AddWithValue("@phone", _phone);
                cmd.Parameters.AddWithValue("@password", HashPsw);
                cmd.Parameters.AddWithValue("@role", "user");
                int result = cmd.ExecuteNonQuery();

                if (result > 0)
                {
                    await DisplayAlert("REQUEST SENT", "Request is in queue for admin approval...", "OK");
                }
                else
                {
                    await DisplayAlert("FAILURE TO SEND", "Failed to send request... Try again...", "OK");
                }
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
        }
    }

    private void SignUpBtn_Clicked(object sender, EventArgs e)
    {
        SendRequest(user.UserId, user.FirstName, user.LastName, user.MiddleInitial, user.Extension, user.Email, user.Phone, user.Password);
    }

    private async void GoToLoginBtn_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new LoginPage());
    }
}