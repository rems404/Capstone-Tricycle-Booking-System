namespace PickMeAppV2.DriverPages;

public partial class BookingOverview : ContentPage
{
    private DriverModelService dms = new DriverModelService();

    public BookingOverview()
    {
        InitializeComponent();
        this.BindingContext = dms;

        OnAppearing();
    }

    protected void OnAppearing()
    {
        DateTime LocalDate = DateTime.Now;

        dms.LoadBookings();
    }
}