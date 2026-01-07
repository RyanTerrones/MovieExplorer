using System.Net.Http;
using System.Text.Json;
using MovieExplorer.Models;

namespace MovieExplorer.Services
{
    // class is responsible for getting movies for the app and now calls the OMDb web API to get live movie data
    public class MovieService
    {
        // reusable HTTP client for making web requests
        private readonly HttpClient _httpClient = new HttpClient();

        // OMDb API key
        private const string ApiKey = "e664df8";

        // gets a list of movies and full details the OMDb API.
        public async Task<List<Movie>> GetMoviesAsync(string searchTerm)
        {
            // if nothing typed, use a broad default
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = "batman";
            }

            string url =
                $"https://www.omdbapi.com/?apikey={ApiKey}&s={Uri.EscapeDataString(searchTerm)}&type=movie";

            try
            {
                // send GET request
                var response = await _httpClient.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                {
                    //fallback if something goes wrong
                    return GetFallbackMovies();
                }

                // read json body
                var json = await response.Content.ReadAsStringAsync();

                // convert json into our helper type
                var result = JsonSerializer.Deserialize<OmdbSearchResponse>(json);

                // if response invalid or empty, fallback
                if (result == null || result.Search == null || result.Search.Count == 0)
                {
                    return GetFallbackMovies();
                }

                // convert OMDb results into our Movie objects
                var movies = new List<Movie>();

                foreach (var item in result.Search)
                {
                    int year = 0;
                    int.TryParse(item.Year, out year);

                    movies.Add(new Movie
                    {
                        Title = item.Title,
                        Year = year,
                        ImdbId = item.imdbID,
                        Genres = "",
                        Director = "",
                        ImdbRating = 0.0,
                        PosterUrl = item.Poster,
                        Plot = "",
                        Emoji = "🎬"
                    });
                }

                return movies;
            }
            catch
            {
                // network or json error -> fallback
                return GetFallbackMovies();
            }
        }

        // gets full details for one movie from OMDb using imdb id
        public async Task<Movie?> GetMovieDetailsAsync(string imdbId)
        {
            // if we don't have an id, nothing to do
            if (string.IsNullOrWhiteSpace(imdbId))
            {
                return null;
            }

            string url =
                $"https://www.omdbapi.com/?apikey={ApiKey}&i={Uri.EscapeDataString(imdbId)}&plot=full";

            try
            {
                // send GET request
                var response = await _httpClient.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                {
                    return null;
                }

                // read json body
                var json = await response.Content.ReadAsStringAsync();

                // convert json into our helper type
                var detail = JsonSerializer.Deserialize<OmdbDetailResponse>(json);

                // if response is bad, return null
                if (detail == null || detail.Response == "False")
                {
                    return null;
                }

                // convert OMDb detail into our Movie object
                int year = 0;
                int.TryParse(detail.Year, out year);

                double rating = 0.0;
                double.TryParse(detail.imdbRating, out rating);

                // create a Movie with the detailed info
                var movie = new Movie
                {
                    Title = detail.Title ?? "",
                    Year = year,
                    Genres = detail.Genre ?? "",
                    Director = detail.Director ?? "",
                    ImdbRating = rating,
                    PosterUrl = detail.Poster ?? "",
                    Plot = detail.Plot ?? "",
                    ImdbId = imdbId,
                    Emoji = "🎬"
                };

                return movie;
            }
            catch
            {
                // on any error just return null (details failed)
                return null;
            }
        }
        

        // this is used as a fallback just incase the web Api call fails
        private List<Movie> GetFallbackMovies()
        {
            return new List<Movie>
    {
        new Movie
        {
            Title = "The Matrix",
            Year = 1999,
            Genres = "Action, Sci-Fi",
            Director = "The Wachowskis",
            ImdbRating = 8.7,
            PosterUrl = "",
            Emoji = "🕶️"
        },
        new Movie
        {
            Title = "Finding Nemo",
            Year = 2003,
            Genres = "Animation, Family",
            Director = "Andrew Stanton",
            ImdbRating = 8.2,
            PosterUrl = "",
            Emoji = "🐠"
        },
        new Movie
        {
            Title = "Inception",
            Year = 2010,
            Genres = "Action, Sci-Fi",
            Director = "Christopher Nolan",
            ImdbRating = 8.8,
            PosterUrl = "",
            Emoji = "🌀"
        }
    }; 
        }

        // helper classes match the OMDb JSON structure
        private class OmdbSearchResponse
        {
            public List<OmdbMovieItem>? Search { get; set; }
            public string? TotalResults { get; set; }
            public string? Response { get; set; }
            public string? Error { get; set; }
        }

        private class OmdbMovieItem
        {
            public string Title { get; set; } = "";
            public string Year { get; set; } = "";
            public string imdbID { get; set; } = "";
            public string Type { get; set; } = "";
            public string Poster { get; set; } = "";
        }

        private class OmdbDetailResponse
        {
            public string? Title { get; set; }
            public string? Year { get; set; }
            public string? Genre { get; set; }
            public string? Director { get; set; }
            public string? imdbRating { get; set; }
            public string? Poster { get; set; }
            public string? Plot { get; set; }
            public string? Response { get; set; }
            public string? Error { get; set; }
        }
    }
}
