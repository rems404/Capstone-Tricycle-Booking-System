using PMA.Modules;

namespace PMA.Admin;

public partial class DashboardSection : ContentView
{
    AdminService ds;
    private Timer _refresh;
    Book book = new Book();

    public DashboardSection()
	{
		InitializeComponent();
        ds = new AdminService();
        BindingContext = ds;

        ds.CountAvailable();
        ds.CountBooked();
        ds.CountPending();
        ds.CountOngoing(); 
        ds.CountCanceled();
        ds.CountCompleted();
        ds.LoadaCurrentBookings();

        _refresh = new Timer(_ =>
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                ds.CountAvailable();
                ds.CountBooked();
                ds.CountPending();
                ds.CountOngoing();
                ds.CountCanceled();
                ds.CountCompleted();
            });
        }, null, TimeSpan.Zero, TimeSpan.FromSeconds(2));
    }
}