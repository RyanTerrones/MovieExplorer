using System.Net.Http;
using System.Text.Json;
using MovieExplorer.Models;

namespace MovieExplorer.Services
{
    //class is responsible for getting movies for the app and now calls the OMDb web API to get live movie data
    public class MovieService
    {
        // Reusable HTTP client for making web requests
        private readonly HttpClient _httpClient = new HttpClient();

        //OMDb API key
        private const string ApiKey = "e664df8";

        //gets a list of movies from the OMDb API.
        public async Task<List<Movie>> GetMoviesAsync()
        {
            // This is the search text we send to OMDb
            string searchTerm = "batman";

            //the OMDb URL
            string url =
                $"https://www.omdbapi.com/?apikey={ApiKey}&s={Uri.EscapeDataString(searchTerm)}&type=movie";

            try
            {
                // Send GET request to the OMDb API
                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    // If something went wrong like no internetit will fall back to hard-coded sample movies
                    return GetFallbackMovies();
                }

                // Read the JSON body as a string
                var json = await response.Content.ReadAsStringAsync();

                //deserialize into a helper class that matches the OMDb JSON structure
                var result = JsonSerializer.Deserialize<OmdbSearchResponse>(json);

                // If the response is invalid or has no Search list, use fallback movies
                if (result == null || result.Search == null || result.Search.Count == 0)
                {
                    return GetFallbackMovies();
                }

                // Map OMDb results into our own Movie objects
                var movies = new List<Movie>();

                foreach (var item in result.Search)
                {
                    int year = 0;
                    int.TryParse(item.Year, out year);

                    movies.Add(new Movie
                    {
                        Title = item.Title,
                        Year = year,
                        Genres = "",
                        Director = "",
                        ImdbRating = 0.0,
                        PosterUrl = item.Poster,
                        Emoji = "🎬" 
                    });
                }

                return movies;
            }
            catch
            {
                //use fallback movies
                return GetFallbackMovies();
            }
        }

        // This is used as a fallback just incase the web Api call fails
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

        //helper classes match the OMDb JSON structure
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
    }
}
