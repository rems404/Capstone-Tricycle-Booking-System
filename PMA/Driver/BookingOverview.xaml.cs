namespace PMA.Driver;

public partial class BookingOverview : ContentPage
{
    BookingViewModel bm;
    private Timer _refresh;

    public BookingOverview()
	{
		InitializeComponent();
        bm = new BookingViewModel();
        BindingContext = bm;

        _refresh = new Timer(_ =>
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                bm.GetLastLogged();
            });
        }, null, TimeSpan.Zero, TimeSpan.FromSeconds(2));
    }
}