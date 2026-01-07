using Microsoft.Maui.Controls;

namespace MovieExplorer.Pages;

public partial class LoginPage : ContentPage
{
    public LoginPage()
    {
        InitializeComponent();
    }

    private void Menu_Clicked(object sender, EventArgs e)
    {
        var flyout = MainFlyoutPage.Current;
        if (flyout != null)
            flyout.IsPresented = !flyout.IsPresented;
    }

    private async void SignIn_Clicked(object sender, EventArgs e)
    {
        await DisplayAlert("Login", "Login successful.", "OK");
    }
}