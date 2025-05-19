using MySql.Data.MySqlClient;
using BCrypt.Net;
using PickMeApp_Admin.Module;
namespace PickMeApp_Admin;

public partial class NewPage1 : ContentPage
{
    private string username;
    private ServerConnection cnnt = new ServerConnection();

    public NewPage1(string loggedInUsername)
    {
        InitializeComponent();
        username = loggedInUsername;
        welcome_lbl.Text = $"Welcome, {username}!";
    }

    private async void EditProfileClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new AdminLandingPage(username));
    }

    // Show the Add Driver popup
    private void AddDriverClicked(object sender, EventArgs e)
    {
        AddDriverPopup.IsVisible = true;
    }

    // Cancel and hide the popup
    private void CancelDriverClicked(object sender, EventArgs e)
    {
        AddDriverPopup.IsVisible = false;
        ClearDriverFields();
    }

    // Save driver to database
    private async void SaveDriverClicked(object sender, EventArgs e)
    {
        String firstName = firstName_tbx.Text;
        String lastName = lastName_tbx.Text;
        String middleInitial = middleInitial_tbx.Text;
        String address = address_tbx.Text;
        String tricycleNumber = tricycleNumber_tbx.Text;
        String phoneNumber = phoneNumber_tbx.Text;

        if(string.IsNullOrWhiteSpace(firstName) ||
            String.IsNullOrWhiteSpace(lastName) ||
            String.IsNullOrWhiteSpace(address) ||
        String.IsNullOrWhiteSpace(tricycleNumber) ||
            String.IsNullOrWhiteSpace(phoneNumber))
            {
            await DisplayAlert("Error", "Please fill in all required fields.", "OK");
            return;
        }
        cnnt.ConnectAdmin();

        using(var cmd = new MySqlCommand("INSERT INTO drivers(firstname, lastname, middle_initial, address, tricycle_number, phone_number) VALUES(@fname, @lname, @mi, @address, @tricycle, @phone)", cnnt.conn))
            {
            cmd.Parameters.AddWithValue("@fname", firstName);
            cmd.Parameters.AddWithValue("@lname", lastName);
            cmd.Parameters.AddWithValue("@mi", middleInitial);
            cmd.Parameters.AddWithValue("@address", address);
            cmd.Parameters.AddWithValue("@tricycle", tricycleNumber);
            cmd.Parameters.AddWithValue("@phone", phoneNumber);

            try
                {
                int result = cmd.ExecuteNonQuery();
                if(result > 0)
                    {
                    await DisplayAlert("Success", "Driver added successfully.", "OK");
                    AddDriverPopup.IsVisible = false;
                    ClearDriverFields();
                }
                else
                    {
                    await DisplayAlert("Error", "Failed to add driver.", "OK");
                }
            }
            catch(Exception ex)
                {
                await DisplayAlert("Error", ex.Message, "Ok");
            }
        }
    }

    // Clear all input fields in the popup
    private void ClearDriverFields()
    {
        firstName_tbx.Text = "";
        lastName_tbx.Text = "";
        middleInitial_tbx.Text = "";
        address_tbx.Text = "";
        tricycleNumber_tbx.Text = "";
        phoneNumber_tbx.Text = "";
    }
}

