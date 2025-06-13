using System.Threading.Tasks;
using PMA.Driver;

namespace PMA
{
    public partial class MainPage : ContentPage
    {

        public MainPage()
        {
            InitializeComponent();
        }

        private async void UserPageBtn_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new LoginPage());
        }

        private async void BookingOverviewBtn_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new BookingOverview());
        }
    }

}
