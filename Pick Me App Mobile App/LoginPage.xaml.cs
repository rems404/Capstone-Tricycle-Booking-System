using MySql.Data.MySqlClient;
using BCrypt.Net;
using Passenger.Modules;

namespace Passenger;

public partial class LoginPage : ContentPage
{
	public ServerConnection cnnt = new ServerConnection();
	public EnDecrypt enDecryptHelper;
	MySqlDataReader rdr;
	public String password;
	public String studId;

	public LoginPage()
	{
		InitializeComponent();
		studId_tbx.TextChanged += (sender, e) => validateField();
		password_tbx.TextChanged += (sender, e) => validateField();
	}

	public void validateField()
	{
		studId = studId_tbx.Text;
		password = password_tbx.Text;

		if (!string.IsNullOrEmpty(studId) && !string.IsNullOrEmpty(password))
		{
			login_btn.IsEnabled = true;
		} else
		{
			login_btn.IsEnabled = false;
		}
	}

    private async void isUser(string studId, string enteredPassword)
    {
        cnnt.connect();

        using (var cmd = new MySqlCommand("SELECT password FROM accountrequests WHERE studId=@studId", cnnt.conn))
        {
            cmd.Parameters.AddWithValue("@studId", studId);

            using (var rdr = cmd.ExecuteReader())
            {
                if (!rdr.HasRows)
                {
                    await DisplayAlert("ERROR", "User not found", "OK");
                    return;
                }

                while (rdr.Read())
                {
                    string storedPsw = rdr.GetString(0).Trim(); 
                    bool isCorrect = BCrypt.Net.BCrypt.Verify(enteredPassword, storedPsw);

                    if (isCorrect)
                    {
                        await Navigation.PushAsync(new LandingPage());
                    }
                    else
                    {
                        await DisplayAlert("INCORRECT", "Wrong password", "OK");
                    }
                }
            }
        }
    }

    async void goToSignUpClicked(object sender, EventArgs e)
	{
		await Navigation.PushAsync(new SignUp());
	}

	async void loginClicked(object sender, EventArgs e)
	{
		isUser(studId, password);
	}
}