using MySql.Data.MySqlClient;
using Passenger.Modules;

namespace Passenger;

public partial class LoginPage : ContentPage
{
	public ServerConnection cnnt = new ServerConnection();

	public LoginPage()
	{
		InitializeComponent();
		username_tbx.TextChanged += (sender, e) => validateField();
		password_tbx.TextChanged += (sender, e) => validateField();
	}

	public void validateField()
	{
		String username = username_tbx.Text;
		String password = password_tbx.Text;

		if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
		{
			login_btn.IsEnabled = true;
		} else
		{
			login_btn.IsEnabled = false;
		}
	}

	private async void isUSer(String username, String password)
	{
		cnnt.connect();
		MySqlCommand cmd = new MySqlCommand();
		cmd.Connection = cnnt.conn;
		cmd.CommandText = "SELECT * FROM users WHERE username=@username AND password=@password";
	}

	async void goToSignUpClicked(object sender, EventArgs e)
	{
		await Navigation.PushAsync(new SignUp());
	}

	async void loginClicked(object sender, EventArgs e)
	{
		await Navigation.PushAsync(new LandingPage());
	}
}