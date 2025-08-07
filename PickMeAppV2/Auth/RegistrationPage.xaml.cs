using MySql.Data.MySqlClient;
using System.Text.RegularExpressions;
using PickMeAppV2.Modules;

namespace PickMeAppV2.ForePages;

public partial class RegistrationPage : ContentPage
{
    User user = new User();
    ServerConnection server = new ServerConnection();

    public RegistrationPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        TbxFname.TextChanged += OnChanged;
        TbxLname.TextChanged += OnChanged;
        TbxMI.TextChanged += OnChanged;
        TbxExtension.TextChanged += OnChanged;
        TbxEmail.TextChanged += OnChanged;
        TbxPhone.TextChanged += OnChanged;
        TbxId.TextChanged += OnChanged;
        TbxPassword.TextChanged += OnChanged;
        TbxConfirm.TextChanged += OnChanged;
        CbxTerms.CheckedChanged += OnChanged;
    }

    private void Clear()
    {
        TbxFname.Text = string.Empty;
        TbxLname.Text = string.Empty;
        TbxMI.Text = string.Empty;
        TbxExtension.Text = string.Empty;
        TbxEmail.Text = string.Empty;
        TbxPhone.Text = string.Empty;
        TbxId.Text = string.Empty;
        TbxPassword.Text = string.Empty;
        TbxConfirm.Text = string.Empty;
    }

    // ----- event handler -----//
    private void OnChanged(object sender, EventArgs e)
    {
        user.FirstName = TbxFname.Text?.Trim();
        user.LastName = TbxLname.Text?.Trim();
        user.MiddleInitial = TbxMI.Text?.Trim();
        user.Extension = TbxExtension.Text?.Trim() ?? string.Empty;

        String EmailPattern = @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$";
        String EmailText = TbxEmail.Text?.Trim();
        user.Email = !string.IsNullOrWhiteSpace(EmailText) && Regex.IsMatch(EmailText, EmailPattern) ? EmailText : string.Empty;

        String PhonePattern = @"^(?:\+63|0)9\d{9}$";
        String PhoneText = TbxPhone.Text?.Trim();
        user.Phone = !string.IsNullOrEmpty(PhoneText) && Regex.IsMatch(PhoneText, PhonePattern) ? PhoneText : string.Empty;


        user.UserId = TbxId.Text?.Trim();
        user.Password = string.Equals(TbxConfirm.Text, TbxPassword.Text) ? TbxConfirm.Text?.Trim() : string.Empty;

        TbxConfirm.IsEnabled = !string.IsNullOrWhiteSpace(TbxPassword.Text);
        BtnNext.IsEnabled = !string.IsNullOrWhiteSpace(user.FirstName) && !string.IsNullOrWhiteSpace(user.LastName) && !string.IsNullOrWhiteSpace(user.MiddleInitial)
                            && !string.IsNullOrWhiteSpace(user.Email) && !string.IsNullOrWhiteSpace(user.Phone);
        BtnSignUp.IsEnabled = !string.IsNullOrWhiteSpace(user.UserId) && !string.IsNullOrWhiteSpace(user.Password) && CbxTerms.IsChecked == true;
    }
    private void BtnNext_Clicked(object sender, EventArgs e)
    {
        FrmAccount.IsVisible = true;
        FrmPersonal.IsVisible = false;
    }

    private void BtnBack_Clicked(object sender, EventArgs e)
    {
        FrmAccount.IsVisible = false;
        FrmPersonal.IsVisible = true;
    }

    private async void BtnSignUp_Clicked(object sender, EventArgs e)
    {
        bool sent = await SendAccoutRequest(user);

        // add sending loader

        switch (sent)
        {
            case true:
                // personalized alert for successfully sending requests
                await DisplayAlert("REQUEST SENT", "Your request is successfully sent and in queue for approval.", "OK");
                Clear();
                await Navigation.PushAsync(new LoginPage());
                break;

            case false:
                await DisplayAlert("EXISTING REQUEST", "You had already sent a request and are in the queue for approval.", "OK");
                break;

            default:
                await DisplayAlert("FAILED", "Failure... Try again...", "OK");
                break;
        }
    }

    private async void BtnLogin_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new LoginPage());
    }

    private async Task<bool> SendAccoutRequest(User user)
    {
        string ExistingRequest = @"SELECT * FROM accountrequests WHERE userid = @userid";
        string AddRequest = @"INSERT INTO accountrequests (userid, lastname, firstname, middleinitial, extension, email, phone, password)
                          VALUES (@userid, @lname, @fname, @mi, @ex, @email, @phone, @password)";
        int affected = 0;

        try
        {
            using var conn = server.GetConnection();
            conn.Open();

            bool exists = false;

            using (var cmd = new MySqlCommand(ExistingRequest, conn))
            {
                cmd.Parameters.AddWithValue("@userid", user.UserId);
                using (var rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        exists = true;
                    }
                }
            }

            if (exists)
            {
                return false;
            }

            string HashPassword = BCrypt.Net.BCrypt.EnhancedHashPassword(user.Password);

            using var addCmd = new MySqlCommand(AddRequest, conn);
            addCmd.Parameters.AddWithValue("@userid", user.UserId);
            addCmd.Parameters.AddWithValue("@lname", user.LastName?.ToLower());
            addCmd.Parameters.AddWithValue("@fname", user.FirstName?.ToLower());
            addCmd.Parameters.AddWithValue("@mi", user.MiddleInitial?.ToLower());
            addCmd.Parameters.AddWithValue("@ex", user.Extension?.ToLower());
            addCmd.Parameters.AddWithValue("@email", user.Email);
            addCmd.Parameters.AddWithValue("@phone", user.Phone);
            addCmd.Parameters.AddWithValue("@password", HashPassword);

            affected = addCmd.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            //await DisplayAlert("ERROR", $"FAILED: {ex.Message}", "OK");
        }

        return affected > 0;
    }
}