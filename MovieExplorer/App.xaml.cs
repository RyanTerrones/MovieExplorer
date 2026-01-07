using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Storage;
using MovieExplorer.Pages;
using MovieExplorer.Services;
using MovieExplorer.Resources.Styles;

namespace MovieExplorer;

public partial class App : Application
{
    public static LibraryService Library { get; private set; } = null!;

    private readonly IServiceProvider _services;

    public App(LibraryService libraryService, IServiceProvider services)
    {
        InitializeComponent();

        Library = libraryService;
        _services = services;

        bool isDark = Preferences.Get("gitflix_darkmode", true);
        ApplyThemeDictionary(isDark);
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        var root = _services.GetRequiredService<MainFlyoutPage>();
        return new Window(root);
    }

    public static void SetRootPage(Page page)
    {
        var win = Current?.Windows?.FirstOrDefault();
        if (win != null)
            win.Page = page;
    }

    public static void SetDarkMode(bool isDark)
    {
        Preferences.Set("gitflix_darkmode", isDark);

        if (Current is not App app)
            return;

        Current.UserAppTheme = isDark ? AppTheme.Dark : AppTheme.Light;

        app.ApplyThemeDictionary(isDark);
    }

    private void ApplyThemeDictionary(bool isDark)
    {
        var merged = Resources.MergedDictionaries;

        var toRemove = merged
            .Where(d => d is ThemeDark || d is ThemeLight)
            .ToList();

        foreach (var d in toRemove)
            merged.Remove(d);

        merged.Add(isDark ? new ThemeDark() : new ThemeLight());
    }
}