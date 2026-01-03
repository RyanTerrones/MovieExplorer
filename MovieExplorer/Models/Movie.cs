using System.Text.Json.Serialization;

namespace MovieExplorer.Models;

//this is a simple data class that represents one movie
public class Movie
{
    public string Title { get; set; } = string.Empty;
    public int Year { get; set; }
    public string Genres { get; set; } = string.Empty;
    public string Director { get; set; } = string.Empty;
    public double ImdbRating { get; set; }

    //id used by OMDb
    public string ImdbId { get; set; } = string.Empty;

    [JsonPropertyName("Poster")]
    public string Poster { get; set; }

    [JsonIgnore]
    public string? PosterUrl
    {
        get => Poster;
        set => Poster = value;
    }

    //URL of the movie poster image from the API
    public string PosterSafe
    {
        get
        {
            if (string.IsNullOrWhiteSpace(Poster) || Poster.Equals("N/A", StringComparison.OrdinalIgnoreCase))
                return "";

            return Poster.Replace("http://", "https://");
        }
    }

    //emoji used as a fallback
    public string Emoji { get; set; } = string.Empty;

    //description
    public string Plot { get; set; } = string.Empty;


}