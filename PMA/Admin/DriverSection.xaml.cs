using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Windows.Input;
using MySql.Data.MySqlClient;
using PMA.Modules;

namespace PMA.Admin;

public partial class DriverSection : ContentView, INotifyPropertyChanged
{
    String server = "server=localhost; uid=root; password=; database=pma";
    Drivers driver = new Drivers();
    AdminService ds;

    public DriverSection()
	{
		InitializeComponent();
        ds = new AdminService();
        BindingContext = ds;


        NewDIdTbx.TextChanged += (sender, e) => Validate();
        NewDFnameTbx.TextChanged += (sender, e) => Validate();
        NewDLnameTbx.TextChanged += (sender, e) => Validate();
        NewDMITbx.TextChanged += (sender, e) => Validate();
        NewDExTbx.TextChanged += (sender, e) => Validate();
        NewDAddressTbx.TextChanged += (sender, e) => Validate();
        NewDPhoneTbx.TextChanged += (sender, e) => Validate();
        NewTNoTbx.TextChanged += (sender, e) => Validate();
        NewTCapacityTbx.TextChanged += (sender, e) => Validate();

        ds.LoadDriver();
        ds.CountDrivers();
    }

    private void Validate()
    {
        // Validate DriverId
        if (string.IsNullOrWhiteSpace(NewDIdTbx.Text))
        {
            NewDIdTbx.Placeholder = "ID is required.";
        }
        else
        {
            driver.DriverId = NewDIdTbx.Text; // Set the DriverId in AdminViewModel directly
        }

        // Validate First Name
        if (string.IsNullOrWhiteSpace(NewDFnameTbx.Text))
        {
            NewDFnameTbx.Placeholder = "First name is required.";
        }
        else
        {
            driver.FirstName = NewDFnameTbx.Text; // Set FirstName in AdminViewModel
        }

        // Validate Last Name
        if (string.IsNullOrWhiteSpace(NewDLnameTbx.Text))
        {
            NewDLnameTbx.Placeholder = "Last name is required.";
        }
        else
        {
            driver.LastName = NewDLnameTbx.Text; // Set LastName in AdminViewModel
        }

        // Validate Middle Initial
        if (string.IsNullOrWhiteSpace(NewDMITbx.Text))
        {
            NewDMITbx.Placeholder = "Middle initial is required.";
        }
        else
        {
            driver.MiddleInitial = NewDMITbx.Text; // Set MiddleInitial in AdminViewModel
        }

        // Set other properties directly in the ViewModel
        driver.Extension = NewDExTbx.Text;
        driver.Address = NewDAddressTbx.Text;
        driver.Phone = NewDPhoneTbx.Text;
        driver.TricycleNo = NewTNoTbx.Text;

        // Validate Capacity
        if (string.IsNullOrWhiteSpace(NewTCapacityTbx.Text))
        {
            NewTCapacityTbx.Placeholder = "Capacity is required.";
        }
        else
        {
            try
            {
                driver.Capacity = int.Parse(NewTCapacityTbx.Text);
            } catch (Exception ex)
            {
                NewTCapacityTbx.Placeholder = "Number...";
            }// Ensure valid integer input
        }

        // Enable Add button only if all fields are valid
        AddBtn.IsEnabled = !string.IsNullOrWhiteSpace(driver.DriverId) && !string.IsNullOrWhiteSpace(driver.FirstName)
                            && !string.IsNullOrWhiteSpace(driver.LastName) && !string.IsNullOrWhiteSpace(driver.MiddleInitial)
                            && !string.IsNullOrWhiteSpace(driver.Address) && !string.IsNullOrWhiteSpace(driver.Phone)
                            && !string.IsNullOrWhiteSpace(driver.TricycleNo) && driver.Capacity > 0;
    }

    private async void DriverDeleteBtn_Clicked(object sender, EventArgs e)
    {
        var button = (Button)sender;
        var driver = button.CommandParameter as Drivers;

        if (driver == null) return;

        bool confirm = await Application.Current.MainPage.DisplayAlert(
            "DELETE DRIVER", $"Delete Driver {driver.DriverId}?", "YES", "NO");

        if (!confirm) return;

        ds.DeleteDriver(driver.DriverId);
        ds.LoadDriver();
        ds.CountDrivers();
    }

    private void NewDriverBtn_Clicked(object sender, EventArgs e)
    {
        NewDriverPopUp.IsVisible = true;
    }

    private async void AddBtn_Clicked(object sender, EventArgs e)
    {
        bool confirm = await Application.Current.MainPage.DisplayAlert("ADD DRIVER", $"Are you sure you want to add {driver.LastName} as a new driver?", "YES", "NO");
        if (!confirm) return;

        var NewDriverInfo = new Drivers
        {
            DriverId = driver.DriverId,
            FirstName = driver.FirstName,
            LastName = driver.LastName,
            MiddleInitial = driver.MiddleInitial,
            Address = driver.Address,
            Phone = driver.Phone,
            TricycleNo = driver.TricycleNo,
            Capacity = driver.Capacity
        };
        int Existing = ds.NewDriver(NewDriverInfo);

        if (Existing == 1)
        {
            NewDriverPopUp.IsVisible = false;
            await Application.Current.MainPage.DisplayAlert("NEW DRIVER ALERT", $"Sucessfully added {driver.LastName}...", "OK");
            ds.LoadDriver();
            ds.CountDrivers();
        } else
        {
            await Application.Current.MainPage.DisplayAlert("ALREADY REGISTERED", $"{driver.DriverId} is already registered.", "OK");
        }
        //ds.NewDriver(NewDriverInfo);
        //NewDriverPopUp.IsVisible = false;
        //await Application.Current.MainPage.DisplayAlert("NEW DRIVER ALERT", $"Sucessfully added {driver.LastName}...", "OK");
        //ds.LoadDriver();
        //ds.CountDrivers();
    }

    private void CancelBtn_Clicked(object sender, EventArgs e)
    {
        NewDriverPopUp.IsVisible = false;
    }

    private void DriverSearchBar_TextChanged(object sender, TextChangedEventArgs e)
    {
        String searchval = DriverSearchBar.Text;

        if (string.IsNullOrWhiteSpace(searchval))
        {
            // If search is cleared, load all drivers (optional)
            ds.LoadDriver();  // Make sure LoadDriver loads all drivers from the database
        }
        else
        {
            // Otherwise, load search results
            ds.LoadSearchDriver(searchval);
        }
    }
}