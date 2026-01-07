using Microsoft.Extensions.Logging;
using MovieExplorer.Pages;
using MovieExplorer.Services;

namespace MovieExplorer;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        // services
        builder.Services.AddSingleton<HttpClient>();
        builder.Services.AddSingleton<LibraryService>();
        builder.Services.AddSingleton<MovieService>();

        // root page
        builder.Services.AddSingleton<MainFlyoutPage>();

        // pages
        builder.Services.AddTransient<MainPage>();
        builder.Services.AddTransient<MoviesPage>();
        builder.Services.AddTransient<MovieDetailsPage>();
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<FiltersPage>();
        builder.Services.AddTransient<Top250Page>();
        builder.Services.AddTransient<WatchlistPage>();
        builder.Services.AddTransient<HistoryPage>();
        builder.Services.AddTransient<FavouritesPage>();
        builder.Services.AddTransient<SettingsPage>();

        return builder.Build();
    }
}
