namespace PMA.Admin;

public partial class AdminLanding : ContentPage
{
    String server = "server=localhost; uid=root; password=; database=pma";

    public AdminLanding(String userid)
	{
		InitializeComponent();
        ContentArea.Content = new DashboardSection();
    }

    private async void LogoutBtn_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

    private void MenuBtn_Clicked(object sender, EventArgs e)
    {
        SideBarLyt.IsVisible = true;
    }

    private void DashboardBtn_Clicked(object sender, EventArgs e)
    {
        ContentArea.Content = new DashboardSection();
    }

    private void DriversSectionBtn_Clicked(object sender, EventArgs e)
    {
        ContentArea.Content = new DriverSection();
    }

    private void CloseBtn_Clicked(object sender, EventArgs e)
    {
        SideBarLyt.IsVisible = false;
    }
}