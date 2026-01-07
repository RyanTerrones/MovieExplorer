using System;
using System.Text.Json.Serialization;

namespace MovieExplorer.Models;

// this is a simple data class that represents one movie
public class Movie
{
    public string Title { get; set; } = string.Empty;
    public int Year { get; set; }
    public string Genres { get; set; } = string.Empty;
    public string Director { get; set; } = string.Empty;
    public double ImdbRating { get; set; }

    [JsonPropertyName("Poster")]
    public string Poster { get; set; } = string.Empty;

    // used by OMDb
    public string ImdbId { get; set; } = string.Empty;

    [JsonIgnore]
    public string? PosterUrl
    {
        get => Poster;
        set => Poster = value ?? string.Empty;
    }

    // URL of the movie poster image from the API (safe for Android)
    [JsonIgnore]
    public string PosterSafe
    {
        get
        {
            if (string.IsNullOrWhiteSpace(Poster) || Poster.Equals("N/A", StringComparison.OrdinalIgnoreCase))
                return string.Empty;

            return Poster.Replace("http://", "https://");
        }
    }

    // if no poster URL, show a placeholder image 
    [JsonIgnore]
    public string PosterSource
    {
        get
        {
            if (!string.IsNullOrWhiteSpace(PosterSafe))
                return PosterSafe;

            return "poster_placeholder.png";
        }
    }

    // emoji used as a fallback
    public string Emoji { get; set; } = "??";

    // description
    public string Plot { get; set; } = string.Empty;
}