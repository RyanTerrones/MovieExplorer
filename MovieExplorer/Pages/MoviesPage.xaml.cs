using System.Collections.ObjectModel;
using MovieExplorer.Models;
using MovieExplorer.Services;

namespace MovieExplorer.Pages;

public partial class MoviesPage : ContentPage
{
    // Service that fetches movies
    private readonly MovieService _movieService = new MovieService();

    // Full set of movies returned by the service
    private List<Movie> _allMovies = new List<Movie>();

    // Movies currently shown in the UI
    private ObservableCollection<Movie> _movies = new ObservableCollection<Movie>();

    public MoviesPage()
    {
        InitializeComponent();

        // Bind the CollectionView in XAML to the observable collection
        MoviesCollectionView.ItemsSource = _movies;
    }

    //called automatically when the page becomes visible and use this to load movies from the service
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        //Only load once
        if (_allMovies.Count == 0)
        {
            //Ask the service for movies
            var movies = await _movieService.GetMoviesAsync();

            //Store them in the master list
            _allMovies = movies;

            //Show them in the UI
            ApplyFilter(TitleSearchBar.Text);
        }
    }

    //Applies a text filter to the movie list and updates the UI collection
    private void ApplyFilter(string searchText)
    {
        searchText = searchText?.Trim() ?? "";
        string lower = searchText.ToLowerInvariant();

        _movies.Clear();

        IEnumerable<Movie> toShow;

        if (string.IsNullOrEmpty(lower))
        {
            toShow = _allMovies;
        }
        else
        {
            toShow = _allMovies.Where(m => m.Title.ToLowerInvariant().Contains(lower));
        }

        foreach (var movie in toShow)
        {
            _movies.Add(movie);
        }
    }

    //Called whenever the text in the SearchBar changes
    private void TitleSearchBar_TextChanged(object sender, TextChangedEventArgs e)
    {
        ApplyFilter(e.NewTextValue);
    }
}