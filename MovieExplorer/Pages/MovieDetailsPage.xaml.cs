using MovieExplorer.Models;
using MovieExplorer.Services;

namespace MovieExplorer.Pages;

public partial class MovieDetailsPage : ContentPage
{
    //movie received from the list page
    private readonly Movie _initialMovie;

    //service used to load full details
    private readonly MovieService _movieService = new MovieService();
    
    public MovieDetailsPage(Movie movie)
    {
        InitializeComponent();

        _initialMovie = movie;

        //show basic info straight away while details load
        Title = movie.Title;

        if (!string.IsNullOrWhiteSpace(movie.PosterUrl))
        {
            PosterImage.Source = movie.PosterUrl;
        }

        TitleLabel.Text = movie.Title;
        YearLabel.Text = movie.Year > 0 ? movie.Year.ToString() : "";
        GenresLabel.Text = "";
        DirectorLabel.Text = "";
        RatingLabel.Text = "IMDB Rating: loading...";
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        //if there's no imdb id,  can't load more details
        if (string.IsNullOrWhiteSpace(_initialMovie.ImdbId))
        {
            return;
        }

        //ask service for full details
        var fullMovie = await _movieService.GetMovieDetailsAsync(_initialMovie.ImdbId);
        if (fullMovie == null)
        {
            return;
        }

        //set binding context so XAML bindings (like Plot) can use it
        BindingContext = fullMovie;

        //update page title and poster if needed
        Title = fullMovie.Title;

        if (!string.IsNullOrWhiteSpace(fullMovie.PosterUrl))
        {
            PosterImage.Source = fullMovie.PosterUrl;
        }

        //update labels with detailed info
        TitleLabel.Text = fullMovie.Title;
        YearLabel.Text = fullMovie.Year > 0 ? fullMovie.Year.ToString() : "";

        if (!string.IsNullOrWhiteSpace(fullMovie.Genres))
        {
            GenresLabel.Text = "Genres: " + fullMovie.Genres;
        }
        else
        {
            GenresLabel.Text = "";
        }

        if (!string.IsNullOrWhiteSpace(fullMovie.Director))
        {
            DirectorLabel.Text = "Director: " + fullMovie.Director;
        }
        else
        {
            DirectorLabel.Text = "";
        }

        if (fullMovie.ImdbRating > 0)
        {
            RatingLabel.Text = "IMDB Rating: " + fullMovie.ImdbRating;
        }
        else
        {
            RatingLabel.Text = "IMDB Rating: N/A";
        }
    }
}