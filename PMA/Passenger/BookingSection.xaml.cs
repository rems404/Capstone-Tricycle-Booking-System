namespace PMA.Passenger;

public partial class BookingSection : ContentView
{
    private string _lastMapHtml = string.Empty;

    private UserViewModel vm = new UserViewModel();
    private Timer _statusRefresh;
    private Timer _driverRefresh;
    private Timer _seatRefresh;

    public BookingSection(string userid)
    {
        InitializeComponent();
        this.BindingContext = vm;

        Initialize(userid);
    }

    private async void Initialize(string userid)
    {
        await vm.LoadUser(userid);
        await vm.LoadDriverInfo();
        await vm.LoadBookingInfo();

        await Application.Current.MainPage.DisplayAlert("DEBUG", $"BookingId: {vm.BookingId}, Status: {vm.Status}", "OK");

        SetSeatSelector();
        HasActiveBooking(); // Now booking info is properly loaded

        _seatRefresh = new Timer(_ =>
        {
            MainThread.BeginInvokeOnMainThread(() => SetSeatSelector());
        }, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));

        _driverRefresh = new Timer(_ =>
        {
            MainThread.BeginInvokeOnMainThread(() => vm.LoadDriverInfo());
        }, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));

        _statusRefresh = new Timer(_ =>
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await vm.LoadBookingInfo(); // ← IMPORTANT: Refresh booking info first
                vm.BookingStatusChecker();
                HasActiveBooking();
            });
        }, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));

        SeatsPcr.SelectedIndexChanged += (sender, e) => Validate();
    }

    private void Validate()
    {
        int SelectedSeats = (int)SeatsPcr.SelectedItem;
        String SelectedLocation = (string)LocationPcr.SelectedItem;

        BookBtn.IsEnabled = SelectedSeats != 0 && !string.IsNullOrWhiteSpace(SelectedLocation);
    }


    private void SetSeatSelector()
    {
        if (vm.AvailableSeats > 0)
        {
            var SeatList = Enumerable.Range(1, vm.AvailableSeats).Reverse().ToList();
            SeatsPcr.ItemsSource = SeatList;
        }
    }

    private void BookingButtonHandler()
    {
        switch (vm.Status)
        {
            case "pending":
                CompletedBtn.IsEnabled = false;
                CancelBtn.IsEnabled = true;
                break;
            case "ongoing":
                CompletedBtn.IsEnabled = true;
                CancelBtn.IsEnabled = false;
                break;
            default:
                CompletedBtn.IsEnabled = false;
                CancelBtn.IsEnabled = false;
                break;
        }
    }

    //public async void HasActiveBooking()
    //{
    //    if (vm.BookingId > 0 && (vm.Status == "ongoing" || vm.Status == "pending"))
    //    {
    //        BookingCard.IsVisible = true;
    //        LandingView.IsVisible = false;

    //        string html = await vm.GetLocationHtml();
    //        MapView.Source = new HtmlWebViewSource
    //        {
    //            Html = html
    //        };

    //        BookingButtonHandler();
    //    }
    //    else
    //    {
    //        BookingCard.IsVisible = false;
    //        LandingView.IsVisible = true;
    //    }
    //}
    public async void HasActiveBooking()
    {
        if (vm.BookingId > 0 && (vm.Status == "ongoing" || vm.Status == "pending"))
        {
            BookingCard.IsVisible = true;
            LandingView.IsVisible = false;

            string html = await vm.GetLocationHtml();
            if (_lastMapHtml != html)
            {
                _lastMapHtml = html;
                MapView.Source = new HtmlWebViewSource
                {
                    Html = html
                };
            }

            BookingButtonHandler();
        }
        else
        {
            BookingCard.IsVisible = false;
            LandingView.IsVisible = true;
        }
    }


    private async void BookBtn_Clicked(object sender, EventArgs e)
    {
        vm.SendNewBooking();
        vm.LoadBookingInfo();

        BookingCard.IsVisible = true;
        LandingView.IsVisible = false;

        string bookingConfirmationMessage = $"Booking successful! Waiting for driver's confirmation...";
        await Application.Current.MainPage.DisplayAlert("Booking Confirmation", bookingConfirmationMessage, "OK");

        string html = await vm.GetLocationHtml();
        MapView.Source = new HtmlWebViewSource
        {
            Html = html
        };
    }

    private async void CancelBtn_Clicked(object sender, EventArgs e)
    {
        bool cancel = await Application.Current.MainPage.DisplayAlert("CANCEL BOOKING", "Are you sure you want to cancel your booking?", "Yes", "No");

        if (!cancel)
        {
            return;
        }
        else
        {
            vm.CancelBooking();
            BookingCard.IsVisible = false;
            LandingView.IsVisible = true;

            await Application.Current.MainPage.DisplayAlert("CANCELLED", $"{vm.FirstName.ToUpper()}, you booking is successfully cancelled.", "OK");
        }
    }

    private async void CompletedBtn_Clicked(object sender, EventArgs e)
    {
        bool confirm = await Application.Current.MainPage.DisplayAlert("DESTINATION REACHED", "Have you finally arrived in your destination?", "Yes", "No");

        if (!confirm)
        {
            return;
        }
        else
        {
            vm.CompletedBooking();
            BookingCard.IsVisible = false;
            LandingView.IsVisible = true;

            await Application.Current.MainPage.DisplayAlert("BOOKING COMPLETED", $"{vm.FirstName.ToUpper()}, thank you for booking!", "OK");
        }
    }
}