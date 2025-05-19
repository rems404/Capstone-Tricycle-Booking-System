
using MySql.Data.MySqlClient;
using BCrypt.Net;
using PickMeApp_Admin.Module;
namespace PickMeApp_Admin;

public partial class AdminLandingPage : ContentPage
{
	public ServerConnection cnnt = new ServerConnection();
	public MySqlDataReader rdr;
	public String username;
    //public String lastname;
    //public String firstname;
    //public String password;

    public AdminLandingPage(string loggedInUsername)
	{
		InitializeComponent();
		username = loggedInUsername;
		LoadAdminProfile();
	}
	private void LoadAdminProfile()
	{

		cnnt.ConnectAdmin();

		using (var cmd = new MySqlCommand("SELECT username, lastname, firstname, password FROM admin WHERE username=@username", cnnt.conn))
		{
			cmd.Parameters.AddWithValue("@username", username);

			using (var rdr = cmd.ExecuteReader())
			{
				if (rdr.HasRows)
				{
					while (rdr.Read())
					{
						username_tbx.Text = rdr.GetString(0);
						lastname_tbx.Text = rdr.GetString(1);
						firstname_tbx.Text = rdr.GetString(2);
						password_tbx.Text = rdr.GetString(3);
					}
				}
			}
		}
	}
	private async void SaveChangesClicked(object sender, EventArgs e)
	{
		string newUsername = username_tbx.Text;
        string newLastName= lastname_tbx.Text;
        string newFirstName = firstname_tbx.Text;
        string newPassword = password_tbx.Text;

		//string hashedPassword = BCrypt.Net.BCrypt.HashPassword(newPassword);

		cnnt.ConnectAdmin();

		using (var cmd = new MySqlCommand("UPDATE admin SET username=@username, lastname=@lastname, firstname=@firstname, password=@password WHERE username=@oldUsername", cnnt.conn))
		{
			cmd.Parameters.AddWithValue("@username", newUsername);
            cmd.Parameters.AddWithValue("@lastname", newLastName);
            cmd.Parameters.AddWithValue("@firstname", newFirstName);
            cmd.Parameters.AddWithValue("@password", newPassword);

            cmd.Parameters.AddWithValue("@oldUsername", username);

			int rowsAffected = cmd.ExecuteNonQuery();
			if (rowsAffected > 0)
			{
				await DisplayAlert("SUCCESS", "Profile updated. Please log in again.", "OK");
				Application.Current.MainPage = new NavigationPage(new MainPage());
			}
			else
			{
				await DisplayAlert("ERROR", "Failed to update profile", "OK");
			}
        }
    }
}