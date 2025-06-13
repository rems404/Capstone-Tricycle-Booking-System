using System.Threading.Tasks;

namespace PMA.Passenger;

public partial class UserLanding : ContentPage
{
    private String UserId;
    public UserLanding(String userid)
	{
		InitializeComponent();
        this.UserId = userid;

        ContentArea.Content = new BookingSection(userid);
    }

    private void MenuBtn_Clicked(object sender, EventArgs e)
    {
        SideBarLyt.IsVisible = true;
    }

    private void CloseBtn_Clicked(object sender, EventArgs e)
    {
        SideBarLyt.IsVisible = false;
    }

    private void BookingBtn_Clicked(object sender, EventArgs e)
    {
        ContentArea.Content = new BookingSection(UserId);
    }

    private async void LogoutBtn_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

    private void HistoryBtn_Clicked(object sender, EventArgs e)
    {
        //ContentArea.Content = new BookingHistory(UserId);
    }

    private void ProfileBtn_Clicked(object sender, EventArgs e)
    {
        ContentArea.Content = new ProfilePage(UserId);
    }
}