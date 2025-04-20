using Passenger.Modules;

namespace Passenger;

public partial class SignUp : ContentPage
{
	public String studId = "";
	public String lastName = "";
	public String firstName = "";
	public String email = "";
	public String phone = "";
	private User user;

	public SignUp()
	{
		InitializeComponent();
		studId_tbx.TextChanged += (sender, e) => validateField();
		lname_tbx.TextChanged += (sender, e) => validateField();
		fname_tbx.TextChanged += (sender, e) => validateField();
		email_tbx.TextChanged += (sender, e) => validateField();
		phone_tbx.TextChanged += (sender, e) => validateField();
	}

	public void validateField()
	{
		studId = studId_tbx.Text;
		lastName = lname_tbx.Text;
		firstName = fname_tbx.Text;
		email = email_tbx.Text;
		phone = phone_tbx.Text;

		if (!string.IsNullOrEmpty(studId) && !string.IsNullOrEmpty(lastName) && !string.IsNullOrEmpty(firstName)
			&& !string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(phone))
		{
			next_btn.IsEnabled = true;
		} else
		{
			next_btn.IsEnabled = false;
		}
	}

	public async void nextClicked(object sender, EventArgs e)
	{
		user = new User(studId, lastName, firstName, email, phone);
		await Navigation.PushAsync(new SignUpTwo(user));
	}

	public async void goToLoginClicked(object sender, EventArgs e)
	{
		await Navigation.PushAsync(new LoginPage());
	}
}