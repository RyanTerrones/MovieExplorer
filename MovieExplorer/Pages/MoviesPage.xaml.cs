using System.Collections.ObjectModel;
using System.Linq;
using MovieExplorer.Models;
using MovieExplorer.Services;

namespace MovieExplorer.Pages;

public partial class MoviesPage : ContentPage
{
    // service that fetches movies
    private readonly MovieService _movieService = new MovieService();

    // full set of movies returned by the service
    private List<Movie> _allMovies = new List<Movie>();

    // movies currently shown in the UI
    private ObservableCollection<Movie> _movies = new ObservableCollection<Movie>();

    private string _lastSearchTerm = "";

    private bool _libraryLoaded;

    private List<Movie> _browseMovies = new();

    public MoviesPage()
    {
        InitializeComponent();

        // bind the CollectionView in XAML to the observable collection
        MoviesCollectionView.ItemsSource = _movies;
        SizeChanged += MoviesPage_SizeChanged;
    }

    private void MoviesPage_SizeChanged(object? sender, EventArgs e)
    {
        // responsive grid that shows phone = 2 columns, wider screens = 4 columns
        if (Width >= 900)
            MoviesGridLayout.Span = 4;
        else if (Width >= 600)
            MoviesGridLayout.Span = 3;
        else
            MoviesGridLayout.Span = 2;
    }

    private void SearchEntry_TextChanged(object sender, TextChangedEventArgs e)
    {
        
    }


    private async Task LoadMoviesFromApiAsync(string searchTerm)
    {
        // if left it empty, fall back to last or default
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            searchTerm = _lastSearchTerm;
        }

        var movies = await _movieService.GetMoviesAsync(searchTerm);

        _lastSearchTerm = searchTerm;

        _allMovies = movies ?? new List<Movie>();

        // reuse existing filter to update the observable collection
        ApplyFilter(TitleSearchBar.Text);
    }

    // called automatically when the page becomes visible and use this to load movies from the service
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (!_libraryLoaded)
        {
            await App.Library.LoadAsync(); 
            _libraryLoaded = true;
        }

        if (_browseMovies.Count == 0)
            await LoadBrowseAsync();
    }

    // applies a text filter to the movie list and updates the UI collection
    private void ApplyFilter(string searchText)
    {
        //protects against null
        if (searchText == null)
        {
            searchText = "";
        }

        string lower = searchText.Trim().ToLowerInvariant();

        _movies.Clear();

        IEnumerable<Movie> toShow;

        if (lower.Length == 0)
        {
            // no search text to show everything
            toShow = _allMovies;
        }
        else
        {
            // filter by title that is case-insensitive
            toShow = _allMovies.Where(m =>
                m.Title != null &&
                m.Title.ToLowerInvariant().Contains(lower));
        }

        foreach (var movie in toShow)
        {
            _movies.Add(movie);
        }
    }

    // called whenever the text in the SearchBar changes
    private void TitleSearchBar_TextChanged(object sender, TextChangedEventArgs e)
    {
        ApplyFilter(e.NewTextValue);
    }

    private async void TitleSearchBar_SearchButtonPressed(object sender, EventArgs e)
{
    await LoadMoviesFromApiAsync(TitleSearchBar.Text);
}

    private async void MoviesCollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        // get the first selected item
        var selected = e.CurrentSelection.FirstOrDefault() as Movie;
        if (selected == null)
        {
            return;
        }

        await App.Library.AddToHistoryAsync(selected);

        // clear selection so tapping the same item again works
        ((CollectionView)sender).SelectedItem = null;

        // navigate to the details page, passing the selected movie
        await Navigation.PushAsync(new MovieDetailsPage(selected));

    }

    private void Hamburger_Clicked(object sender, EventArgs e)
    {
        MainFlyoutPage.Current?.ToggleFlyout();
    }

    private void Favourites_Clicked(object sender, EventArgs e)
    {
        MainFlyoutPage.Current?.NavigateTo(new FavouritesPage());
    }

    private void Watchlist_Clicked(object sender, EventArgs e)
    {
        MainFlyoutPage.Current?.NavigateTo(new WatchlistPage());
    }

    private void History_Clicked(object sender, EventArgs e)
    {
        MainFlyoutPage.Current?.NavigateTo(new HistoryPage());
    }

    private async Task LoadBrowseAsync()
    {
        string[] seeds =
        {
        "avengers", "batman", "star wars", "harry potter",
        "spider man", "lord of the rings", "mission impossible", "fast"
    };

        var tasks = seeds.Select(s => _movieService.GetMoviesAsync(s));
        var results = await Task.WhenAll(tasks);

        _browseMovies = results
            .Where(r => r != null)
            .SelectMany(r => r!)
            .GroupBy(m => m.ImdbId)
            .Select(g => g.First())
            .ToList();

        _allMovies = _browseMovies;
        ApplyFilter(TitleSearchBar.Text);
    }

    private async void MenuButton_Clicked(object sender, EventArgs e)
    {
        var action = await DisplayActionSheet("GitFlix", "Cancel", null, "Settings", "Login");

        if (action == "Settings")
            await Navigation.PushAsync(new SettingsPage());

        if (action == "Login")
            await Navigation.PushAsync(new LoginPage());
    }

    private MovieFilters _filters = new();

    private async void FiltersButton_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new FiltersPage(_filters, f =>
        {
            _filters = f;
            ApplyFilter(TitleSearchBar.Text);
        }));
    }
}