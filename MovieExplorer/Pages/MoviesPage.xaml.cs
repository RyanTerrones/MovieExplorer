using System.Collections.ObjectModel;
using MovieExplorer.Models;

namespace MovieExplorer.Pages;

public partial class MoviesPage : ContentPage
{
    //master list of all movies
    private List<Movie> _allMovies = new List<Movie>();

    // When we change this collection, the UI updates automatically.
    private ObservableCollection<Movie> _movies = new ObservableCollection<Movie>();

    public MoviesPage()
    {
        InitializeComponent();

        //bind the UI list (CollectionView) to the observable collection.
        MoviesCollectionView.ItemsSource = _movies;

        //load some sample movies so we can see and test the page. (for now)
        LoadSampleMovies();
        ApplyFilter(""); // Start with "no filter" so all movies appear
    }

    private void LoadSampleMovies()
    {
        _allMovies.Clear();

        _allMovies.Add(new Movie
        {
            Title = "The Matrix",
            Year = 1999,
            Genres = "Action, Sci-Fi",
            Director = "The Wachowskis",
            ImdbRating = 8.7,
            Emoji = "🕶️"
        });

        _allMovies.Add(new Movie
        {
            Title = "Finding Nemo",
            Year = 2003,
            Genres = "Animation, Family",
            Director = "Andrew Stanton",
            ImdbRating = 8.2,
            Emoji = "🐠"
        });

        _allMovies.Add(new Movie
        {
            Title = "Inception",
            Year = 2010,
            Genres = "Action, Sci-Fi",
            Director = "Christopher Nolan",
            ImdbRating = 8.8,
            Emoji = "🌀"
        });
    }

    private void ApplyFilter(string searchText)
    {
        // Normalise the search text (null-safe and case-insensitive).
        searchText = searchText?.Trim() ?? "";
        string lower = searchText.ToLowerInvariant();

        // Clear the UI collection.
        _movies.Clear();

        //If search text is empty -> show all movies.
        //Otherwise -> only movies whose Title contains the text.
        IEnumerable<Movie> toShow;

        if (string.IsNullOrEmpty(lower))
        {
            toShow = _allMovies;
        }
        else
        {
            toShow = _allMovies.Where(m => m.Title.ToLowerInvariant().Contains(lower));
        }

        // Add the filtered movies to the observable collection.
        foreach (var movie in toShow)
        {
            _movies.Add(movie);
        }
    }

    private void TitleSearchBar_TextChanged(object sender, TextChangedEventArgs e)
    {
        // e.NewTextValue is the current text in the SearchBar.
        ApplyFilter(e.NewTextValue);
    }
}