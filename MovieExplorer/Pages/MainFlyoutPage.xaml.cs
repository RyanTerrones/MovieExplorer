namespace MovieExplorer.Pages;

public partial class MainFlyoutPage : FlyoutPage
{
    public static MainFlyoutPage? Current { get; private set; }

    public MainFlyoutPage()
    {
        InitializeComponent();

        Current = this;
        FlyoutLayoutBehavior = FlyoutLayoutBehavior.Popover;
        IsPresented = false;
    }

    public void ToggleFlyout() => IsPresented = !IsPresented;

    public async void NavigateTo(Page page)
    {
        // ensure Detail is a NavigationPage
        if (Detail is not NavigationPage nav)
        {
            nav = new NavigationPage(new MoviesPage());
            NavigationPage.SetHasNavigationBar(nav, false);
            Detail = nav;
            IsPresented = false;
        }

        // if Home is requested, return to root instead of stacking
        if (page is MoviesPage)
        {
            // if root isn't MoviesPage (rare), force it
            var root = nav.Navigation.NavigationStack.FirstOrDefault();
            if (root is not MoviesPage && root != null)
            {
                nav.Navigation.InsertPageBefore(new MoviesPage(), root);
            }

            await nav.Navigation.PopToRootAsync(animated: false);
        }
        else
        {
            // avoid pushing duplicates of the same page type on top
            var top = nav.Navigation.NavigationStack.LastOrDefault();
            if (top == null || top.GetType() != page.GetType())
                await nav.Navigation.PushAsync(page, animated: true);
        }
    }

    private void Home_Clicked(object sender, EventArgs e) => NavigateTo(new MoviesPage());
    private void Top250_Clicked(object sender, EventArgs e) => NavigateTo(new Top250Page());
    private void Settings_Clicked(object sender, EventArgs e) => NavigateTo(new SettingsPage());
    private void Login_Clicked(object sender, EventArgs e) => NavigateTo(new LoginPage());
}