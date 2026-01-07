using System.Text.Json;
using Microsoft.Maui.Storage;
using MovieExplorer.Models;

namespace MovieExplorer.Services;

public class LibraryService
{
    // this type name + binds these to XAML
    public class MovieMini
    {
        public string ImdbId { get; set; } = "";
        public string Title { get; set; } = "";
        public string Poster { get; set; } = "";
        public string Year { get; set; } = "";

        // XAML uses PosterSafe
        public string PosterSafe
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Poster) ||
                    Poster.Equals("N/A", StringComparison.OrdinalIgnoreCase))
                    return "";

                // force https (Android can block http images)
                return Poster.Replace("http://", "https://");
            }
        }
    }

    private const string KeyFavourites = "library_favourites_v1";
    private const string KeyWatchlist = "library_watchlist_v1";
    private const string KeyHistory = "library_history_v1";

    private readonly object _lock = new();

    private List<MovieMini> _favourites = new();
    private List<MovieMini> _watchlist = new();
    private List<MovieMini> _history = new();

    private bool _loaded;

    public Task LoadAsync()
    {
        lock (_lock)
        {
            _favourites = LoadList(KeyFavourites);
            _watchlist = LoadList(KeyWatchlist);
            _history = LoadList(KeyHistory);
            _loaded = true;
        }

        return Task.CompletedTask;
    }

    private void EnsureLoaded()
    {
        if (_loaded) return;

        LoadAsync().GetAwaiter().GetResult();
    }

    // get lists
    public List<MovieMini> GetFavourites()
    {
        EnsureLoaded();
        lock (_lock) return _favourites.ToList();
    }

    public List<MovieMini> GetWatchList()
    {
        EnsureLoaded();
        lock (_lock) return _watchlist.ToList();
    }

    public List<MovieMini> GetHistory()
    {
        EnsureLoaded();
        lock (_lock) return _history.ToList();
    }

    // checks
    public bool IsFavourite(string imdbId)
    {
        if (string.IsNullOrWhiteSpace(imdbId)) return false;
        EnsureLoaded();
        lock (_lock) return _favourites.Any(x => x.ImdbId == imdbId);
    }

    public bool IsWatchlisted(string imdbId)
    {
        if (string.IsNullOrWhiteSpace(imdbId)) return false;
        EnsureLoaded();
        lock (_lock) return _watchlist.Any(x => x.ImdbId == imdbId);
    }

    // toggle Favourite
    public Task ToggleFavouriteAsync(MovieMini movie)
        => ToggleFavouriteAsync(movie.ImdbId, movie.Title, movie.Poster, movie.Year);

    public Task ToggleFavouriteAsync(Movie movie)
        => ToggleFavouriteAsync(ToMini(movie));

    public Task ToggleFavouriteAsync(string imdbId, string title, string poster, string year)
    {
        if (string.IsNullOrWhiteSpace(imdbId)) return Task.CompletedTask;
        EnsureLoaded();

        lock (_lock)
        {
            var existing = _favourites.FirstOrDefault(x => x.ImdbId == imdbId);
            if (existing != null)
            {
                _favourites.Remove(existing);
            }
            else
            {
                _favourites.Insert(0, new MovieMini
                {
                    ImdbId = imdbId,
                    Title = title ?? "",
                    Poster = poster ?? "",
                    Year = year ?? ""
                });
            }

            SaveList(KeyFavourites, _favourites);
        }

        return Task.CompletedTask;
    }

    // toggle Watchlist
    public Task ToggleWatchlistAsync(MovieMini movie)
        => ToggleWatchlistAsync(movie.ImdbId, movie.Title, movie.Poster, movie.Year);

    public Task ToggleWatchlistAsync(Movie movie)
        => ToggleWatchlistAsync(ToMini(movie));

    public Task ToggleWatchlistAsync(string imdbId, string title, string poster, string year)
    {
        if (string.IsNullOrWhiteSpace(imdbId)) return Task.CompletedTask;
        EnsureLoaded();

        lock (_lock)
        {
            var existing = _watchlist.FirstOrDefault(x => x.ImdbId == imdbId);
            if (existing != null)
            {
                _watchlist.Remove(existing);
            }
            else
            {
                _watchlist.Insert(0, new MovieMini
                {
                    ImdbId = imdbId,
                    Title = title ?? "",
                    Poster = poster ?? "",
                    Year = year ?? ""
                });
            }

            SaveList(KeyWatchlist, _watchlist);
        }

        return Task.CompletedTask;
    }

    // history 
    public Task AddToHistoryAsync(MovieMini movie)
        => AddToHistoryAsync(movie.ImdbId, movie.Title, movie.Poster, movie.Year);

    public Task AddToHistoryAsync(Movie movie)
        => AddToHistoryAsync(ToMini(movie));

    public Task AddToHistoryAsync(string imdbId, string title, string poster, string year)
    {
        if (string.IsNullOrWhiteSpace(imdbId)) return Task.CompletedTask;
        EnsureLoaded();

        lock (_lock)
        {
            // move-to-top behavior
            _history.RemoveAll(x => x.ImdbId == imdbId);

            _history.Insert(0, new MovieMini
            {
                ImdbId = imdbId,
                Title = title ?? "",
                Poster = poster ?? "",
                Year = year ?? ""
            });

            // keep history from growing forever
            const int max = 200;
            if (_history.Count > max)
                _history = _history.Take(max).ToList();

            SaveList(KeyHistory, _history);
        }

        return Task.CompletedTask;
    }

    public Task RemoveHistoryAsync(string imdbId) => RemoveFromHistoryAsync(imdbId);

    public Task RemoveFromHistoryAsync(string imdbId)
    {
        if (string.IsNullOrWhiteSpace(imdbId)) return Task.CompletedTask;
        EnsureLoaded();

        lock (_lock)
        {
            _history.RemoveAll(x => x.ImdbId == imdbId);
            SaveList(KeyHistory, _history);
        }

        return Task.CompletedTask;
    }

    public Task RemoveFavouriteAsync(string imdbId)
    {
        if (string.IsNullOrWhiteSpace(imdbId)) return Task.CompletedTask;
        EnsureLoaded();

        lock (_lock)
        {
            _favourites.RemoveAll(x => x.ImdbId == imdbId);
            SaveList(KeyFavourites, _favourites);
        }

        return Task.CompletedTask;
    }

    public Task RemoveWatchlistAsync(string imdbId)
    {
        if (string.IsNullOrWhiteSpace(imdbId)) return Task.CompletedTask;
        EnsureLoaded();

        lock (_lock)
        {
            _watchlist.RemoveAll(x => x.ImdbId == imdbId);
            SaveList(KeyWatchlist, _watchlist);
        }

        return Task.CompletedTask;
    }

    public Task ClearFavouritesAsync()
    {
        EnsureLoaded();
        lock (_lock)
        {
            _favourites.Clear();
            SaveList(KeyFavourites, _favourites);
        }
        return Task.CompletedTask;
    }

    public Task ClearWatchlistAsync()
    {
        EnsureLoaded();
        lock (_lock)
        {
            _watchlist.Clear();
            SaveList(KeyWatchlist, _watchlist);
        }
        return Task.CompletedTask;
    }

    public Task ClearHistoryAsync()
    {
        EnsureLoaded();
        lock (_lock)
        {
            _history.Clear();
            SaveList(KeyHistory, _history);
        }
        return Task.CompletedTask;
    }

    // helpers
    private static MovieMini ToMini(Movie m)
    {
        return new MovieMini
        {
            ImdbId = m.ImdbId ?? "",
            Title = m.Title ?? "",
            Poster = m.Poster ?? "",
            Year = m.Year.ToString()
        };
    }

    private static List<MovieMini> LoadList(string key)
    {
        try
        {
            var json = Preferences.Get(key, "[]");
            return JsonSerializer.Deserialize<List<MovieMini>>(json) ?? new List<MovieMini>();
        }
        catch
        {
            return new List<MovieMini>();
        }
    }

    private static void SaveList(string key, List<MovieMini> list)
    {
        try
        {
            var json = JsonSerializer.Serialize(list);
            Preferences.Set(key, json);
        }
        catch
        {
            //blank
        }
    }
}