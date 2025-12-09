namespace MovieExplorer.Models 
{
    //simple data class that represents ONE movie.
    public class Movie
    {
        public string Title { get; set; } = string.Empty;
        public int Year { get; set; }
        public string Genres { get; set; } = string.Empty;
        public string Director { get; set; } = string.Empty;
        public double ImdbRating { get; set; }
        public string Emoji { get; set; } = string.Empty;
    }
}