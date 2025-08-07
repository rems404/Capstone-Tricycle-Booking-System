using MySql.Data.MySqlClient;
using PickMeAppV2.Modules;
using PickMeAppV2.UserPages;
using PickMeAppV2.AdminPages;

namespace PickMeAppV2.ForePages;

public partial class LoginPage : ContentPage
{
    User user = new User();
    ServerConnection server = new ServerConnection();
    EmailService es = new EmailService();

    public LoginPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        TbxId.TextChanged += OnTextChanged;
        TbxPassword.TextChanged += OnTextChanged;
    }

    private void OnTextChanged(object sender, EventArgs e)
    {
        user.UserId = TbxId.Text?.Trim();
        user.Password = TbxPassword.Text?.Trim();

        BtnLogin.IsEnabled = !string.IsNullOrWhiteSpace(user.UserId) && !string.IsNullOrWhiteSpace(user.Password);
    }

    private async void BtnLogin_Clicked(object sender, EventArgs e)
    {
        VisualStateManager.GoToState(BtnLogin, "Pressed");

        String role = await IsUser(user.UserId, user.Password);

        switch (role)
        {
            case "user":
                // store session
                await Navigation.PushAsync(new UserLanding(user.UserId));
                TbxId.Text = TbxPassword.Text = string.Empty;
                break;

            case "admin":
                // store session
                await Navigation.PushAsync(new AdminLanding());
                TbxId.Text = TbxPassword.Text = string.Empty;
                break;

            case "" :
                GlobalError.Text = "User not found or incorrect password...";
                break;

            default:
                break;
        }
    }

    private async Task<string> IsUser(String UserId, String password)
    {
        String role = string.Empty;
        String IsUserQuery = "SELECT password, role FROM users WHERE userid = @id";

        try
        {
            using var conn = server.GetConnection();
            conn.Open();

            using var cmd = new MySqlCommand(IsUserQuery, conn);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@id", UserId);

            using var rdr = cmd.ExecuteReader();
            if (rdr.Read())
            {
                String StoredPsw = rdr.GetString("password");
                bool IsCorrectPsw = BCrypt.Net.BCrypt.EnhancedVerify(password, StoredPsw);

                if (IsCorrectPsw)
                {
                    role = rdr.GetString("role");
                }
            }
        }
        catch (Exception e)
        {
            role = e.ToString();
        }

        return role;
    }

    private async void BtnForgotPsw_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new ResetPasswordPage());
    }

    private async void BtnCreate_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new RegistrationPage());
    }
}