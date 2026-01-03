using MovieExplorer.Pages;

namespace MovieExplorer;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        //root of the app is a NavigationPage that starts on MoviesPage
        MainPage = new NavigationPage(new MoviesPage());
    }
}