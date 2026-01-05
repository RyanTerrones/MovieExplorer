using MovieExplorer.Pages;
using MovieExplorer.Services;

namespace MovieExplorer;

public partial class App : Application
{
    // I keep one shared library instance for the app.
    public static LibraryService Library { get; } = new LibraryService();


    public App()
    {
        InitializeComponent();

        //root of the app is a NavigationPage that starts on MoviesPage
        MainPage = new NavigationPage(new MoviesPage());
    }
}