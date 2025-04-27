using MySql.Data.MySqlClient;
using Passenger.Modules;

namespace Passenger;

public partial class SignUp : ContentPage
{
	public String studId = "";
	public String lastName = "";
	public String firstName = "";
	public String email = "";
	public String phone = "";
	public String password = "";
	private User user;
	public ServerConnection cnnt = new ServerConnection();
    public EnDecrypt encryptHelper = new EnDecrypt();

    public SignUp()
	{
		InitializeComponent();
		studId_tbx.TextChanged += (sender, e) => validateField();
		lname_tbx.TextChanged += (sender, e) => validateField();
		fname_tbx.TextChanged += (sender, e) => validateField();
		email_tbx.TextChanged += (sender, e) => validateField();
		phone_tbx.TextChanged += (sender, e) => validateField();
		psw_tbx.TextChanged += (sender, e) => validateField();
	}

	public void validateField()
	{
		studId = studId_tbx.Text;
		lastName = lname_tbx.Text;
		firstName = fname_tbx.Text;
		email = email_tbx.Text;
		phone = phone_tbx.Text;
		password = psw_tbx.Text;

		if (studId != null && lastName != null && firstName != null && email != null && password != null)
		{
            signUp_btn.IsEnabled = true;
		}
		else
		{
            signUp_btn.IsEnabled = false;
		}
	}

	public async void isNewUser(String studId, String lastName, String firstName, String email, String phone, String password)
	{
        String hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
        cnnt.connect();
		MySqlCommand cmd = new MySqlCommand();
		cmd.Connection = cnnt.conn;
		cmd.CommandText = "INSERT INTO accountrequests (studId, lastName, firstName, email, phone, password) VALUES (@studId, @lastName, @firstName, @email, @phone, @password)";
		using (cnnt.conn)
		{

            cmd.Parameters.AddWithValue("@studId", studId);
			cmd.Parameters.AddWithValue("@lastName", lastName);
			cmd.Parameters.AddWithValue("@firstName", firstName);
			cmd.Parameters.AddWithValue("@email", email);
			cmd.Parameters.AddWithValue("@phone", phone);
			cmd.Parameters.AddWithValue("@password", hashedPassword);
			int result = cmd.ExecuteNonQuery();

			if (result > 0)
			{
				await Navigation.PushAsync(new LoginPage());
			}
		}

	}


    public void signUpClicked(object sender, EventArgs e)
	{
		isNewUser(studId, lastName, firstName, email, phone, password);
	}

	public async void goToLoginClicked(object sender, EventArgs e)
	{
		await Navigation.PushAsync(new LoginPage());
	}
}