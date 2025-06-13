namespace PMA.Passenger;

public partial class ProfilePage : ContentView
{
    private UserViewModel vm = new UserViewModel();

    public ProfilePage(String userid)
	{
		InitializeComponent();
        this.BindingContext = vm;

        vm.LoadUser(userid);
    }

    private async void UpdateProfile()
    {
        if (!string.IsNullOrWhiteSpace(NewPasswordTbx.Text))
        {
            vm.Password = NewPasswordTbx.Text;
            vm.PasswordUpdate();
            await Application.Current.MainPage.DisplayAlert("PASSWORD CHANGED", "Password successfully changed.", "OK");
        }
        else
        {
            vm.ProfileUpdate();
            await Application.Current.MainPage.DisplayAlert("UPDATED", "Your profile has been updated.", "OK");
        }
    }

    private async void SaveBtn_Clicked(object sender, EventArgs e)
    {
        bool ApplyChanges = await Application.Current.MainPage.DisplayAlert("APPLY CHANGES", "Are you sure you want to apply the changes to your profile?", "APPLY", "CANCEL");

        if (ApplyChanges)
        {
            UpdateProfile();
        }
        else
        {
            return;
        }
    }
}