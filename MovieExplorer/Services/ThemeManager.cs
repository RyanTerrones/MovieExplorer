using System;
using System.Linq;

namespace MovieExplorer;

public static class ThemeManager
{
    private const string LightTheme = "Resources/Styles/ThemeLight.xaml";
    private const string DarkTheme = "Resources/Styles/ThemeDark.xaml";

    public static void ApplyTheme(bool dark)
    {
        if (Application.Current == null) return;

        var md = Application.Current.Resources.MergedDictionaries;

        // remove existing theme dictionary
        var existing = md.FirstOrDefault(d =>
            d.Source != null &&
            (d.Source.OriginalString.EndsWith("ThemeLight.xaml", StringComparison.OrdinalIgnoreCase) ||
             d.Source.OriginalString.EndsWith("ThemeDark.xaml", StringComparison.OrdinalIgnoreCase)));

        if (existing != null)
            md.Remove(existing);

        md.Add(new ResourceDictionary
        {
            Source = new Uri(dark ? DarkTheme : LightTheme, UriKind.Relative)
        });

        Application.Current.UserAppTheme = dark ? AppTheme.Dark : AppTheme.Light;
    }
}
