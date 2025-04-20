using MySql.Data.MySqlClient;
using Passenger.Modules;

namespace Passenger;

public partial class SignUpTwo : ContentPage
{
	public ServerConnection cnnt = new ServerConnection();
	private User user;
	private EnDecrypt encryptHelper;

	public SignUpTwo(User user)
	{
		InitializeComponent();
		this.user = user;
		this.encryptHelper = new EnDecrypt();
		username_tbx.TextChanged += (sender, e) => validateField();
		password_tbx.TextChanged += (sender, e) => validateField();
	}

	public void validateField()	
	{
		user.username = username_tbx.Text;
		user.password = password_tbx.Text;

		if (!string.IsNullOrEmpty(user.username) && !string.IsNullOrEmpty(user.password))
		{
			signUp_btn.IsEnabled = true;
		} else
		{
			signUp_btn.IsEnabled = false;
		}
	}

	public async void isNewUser(String studId, String lastName, String firstName, String email, String phone, String username, String password)
	{
		cnnt.connect();
		MySqlCommand cmd = new MySqlCommand();
		cmd.Connection = cnnt.conn;
		cmd.CommandText = "INSERT INTO accountrequests (studId, lastName, firstName, email, phone, username, password) VALUES (@studId, @lastName, @firstName, @email, @phone, @username, @password)";
		using (cnnt.conn)
		{
			cmd.Parameters.AddWithValue("@studId", studId);
			cmd.Parameters.AddWithValue("@lastName", lastName);
			cmd.Parameters.AddWithValue("@firstName", firstName);
			cmd.Parameters.AddWithValue("@email", email);
            cmd.Parameters.AddWithValue("@phone", phone);
            cmd.Parameters.AddWithValue("@username", username);
            cmd.Parameters.AddWithValue("@password", password);
			int result = cmd.ExecuteNonQuery();

			if (result > 0)
			{
				await Navigation.PushAsync(new LoginPage());
			}
        }
	}

    void signUpClicked(object sender, EventArgs e)
    {
		String hashedPassword = encryptHelper.hashPassword(user.password);
        isNewUser(user.studId, user.lastName, user.firstName, user.email, user.phone, user.username, hashedPassword);
    }

    // redirects user to login page when login_btn is clicked
    public async void goToLoginClicked(object sender, EventArgs e)
	{
		await Navigation.PushAsync(new LoginPage());
	}
}