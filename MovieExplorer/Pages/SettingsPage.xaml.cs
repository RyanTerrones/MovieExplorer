using System;
using Microsoft.Maui.Storage;

namespace MovieExplorer.Pages;

public partial class SettingsPage : ContentPage
{
    // keeping paths/URIs
    private const string ThemeKey = "gitflix_darkmode";
    private static readonly Uri ThemeDarkUri = new("Resources/Styles/ThemeDark.xaml", UriKind.Relative);
    private static readonly Uri ThemeLightUri = new("Resources/Styles/ThemeLight.xaml", UriKind.Relative);

    private bool _ready;

    public SettingsPage()
    {
        InitializeComponent();

        var isDark = Preferences.Get(ThemeKey, true);

        _ready = false;
        DarkModeSwitch.IsToggled = isDark;

        ApplyTheme(isDark);

        _ready = true;
    }

    private void DarkModeSwitch_Toggled(object sender, ToggledEventArgs e)
    {
        if (!_ready) return;

        App.SetDarkMode(e.Value);
    }

    private void Menu_Clicked(object sender, EventArgs e)
    {
        // if your app root is a FlyoutPage (MainFlyoutPage usually is):
        if (Application.Current?.MainPage is FlyoutPage flyout)
        {
            flyout.IsPresented = true;
            return;
        }

        // if you're using Shell:
        if (Shell.Current is not null)
        {
            Shell.Current.FlyoutIsPresented = true;
            return;
        }
    }

    private static void ApplyTheme(bool dark)
    {
        _ = ThemeDarkUri;
        _ = ThemeLightUri;

        // Save preference
        Preferences.Set(ThemeKey, dark);

        App.SetDarkMode(dark);
    }
}