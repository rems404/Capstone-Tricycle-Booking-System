using MySql.Data.MySqlClient;
using PickMeAppV2.Modules;
using static System.Runtime.CompilerServices.RuntimeHelpers;

namespace PickMeAppV2.ForePages;

public partial class ResetPasswordPage : ContentPage
{
    ServerConnection server = new ServerConnection();
    EmailService es = new EmailService();

    private String UserId;
    private int code;
    private String password;

    public ResetPasswordPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        TbxId.TextChanged += OnTextChanged;
        TbxCode.TextChanged += OnTextChanged;
        TbxNew.TextChanged += OnTextChanged;
        TbxConfirm.TextChanged += OnTextChanged;
    }

    private void OnTextChanged(object sender, EventArgs e)
    {
        UserId = TbxId.Text?.Trim();
        BtnSend.IsEnabled = !string.IsNullOrWhiteSpace(UserId);

        code = int.TryParse(TbxCode.Text, out int ParseCode) ? ParseCode : 0;
        BtnVerify.IsEnabled = code != 0;

        password = string.Equals(TbxNew.Text, TbxConfirm.Text) ? TbxConfirm.Text : string.Empty;
        TbxConfirm.IsEnabled = !string.IsNullOrWhiteSpace(TbxNew.Text);
        BtnSubmit.IsEnabled = !string.IsNullOrWhiteSpace(password);
    }

    private async void BtnCancel_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

    private async void BtnSend_Clicked(object sender, EventArgs e)
    {
        bool sent = await es.ResetPasswordService(UserId);

        // add sending message loader

        if (!sent)
        {
            await DisplayAlert("FAILED", "Failed to send a reset code to your email. Try again.", "OK");
            return;
        }

        FrmVerification.IsVisible = true;
        FrmUserId.IsVisible = false;
    }

    private async void BtnVerify_Clicked(object sender, EventArgs e)
    {
        bool valid = await VerifyCode(code, UserId);

        // add verifying code loader

        if (!valid)
        {
            ErrorMsg.IsVisible = true;
            ErrorMsg.Text = "You have entered an invalid or expired code.";
            return;
        }

        FrmNewPassword.IsVisible = true;
        FrmVerification.IsVisible = false;
    }

    private async void BtnSubmit_Clicked(object sender, EventArgs e)
    {
        bool changed = await SubmitNewPassword(UserId, password);

        // add changing password loader

        if (!changed)
        {
            ErrorMsg.IsVisible = true;
            ErrorMsg.Text = "Failed to change password. Try again.";
            return;
        }

        await DisplayAlert("PASSWORD UPDATED", "You have successfully updated your password.", "OK");
        await Navigation.PopAsync();
    }

    private async Task<bool> VerifyCode(int code, String UserId)
    {
        bool IsValid = false;
        String query = "SELECT token FROM resetlinks WHERE userid = @userid AND token = @code AND expiry > NOW()";

        try
        {
            using var conn = server.GetConnection();
            conn.Open();

            using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@userid", UserId);
            cmd.Parameters.AddWithValue("@code", code);

            using var rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                IsValid = true;
            }
        }
        catch (Exception ex)
        {
            ErrorMsg.IsVisible = true;
            ErrorMsg.Text = ex.Message;
        }

        return IsValid;
    }

    private async Task<bool> SubmitNewPassword(String UserId, String password)
    {
        int affected = 0;
        String HashPsw = BCrypt.Net.BCrypt.EnhancedHashPassword(password);
        String query = "UPDATE users SET password = @psw WHERE userid = @id";

        try
        {
            using var conn = server.GetConnection();
            conn.Open();

            using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@psw", HashPsw);
            cmd.Parameters.AddWithValue("@id", UserId);

            affected = cmd.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            ErrorMsg.IsVisible = true;
            ErrorMsg.Text = ex.Message;
        }

        return affected > 0;
    }
}