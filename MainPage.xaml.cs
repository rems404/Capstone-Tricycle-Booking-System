namespace Passenger
{
    public partial class MainPage : ContentPage
    {

        public MainPage()
        {
            InitializeComponent();
        }

        public async void goToSignUpClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SignUp());
        }
    }

}
