namespace MovieExplorer.Models
{
    // This is a simple data class that represents one movie
    public class Movie
    {
        public string Title { get; set; } = string.Empty;
        public int Year { get; set; }
        public string Genres { get; set; } = string.Empty;
        public string Director { get; set; } = string.Empty;
        public double ImdbRating { get; set; }

        // URL of the movie poster image from the API
        public string PosterUrl { get; set; } = string.Empty;

        // Emoji we use as a fallback
        public string Emoji { get; set; } = string.Empty;
    }
}