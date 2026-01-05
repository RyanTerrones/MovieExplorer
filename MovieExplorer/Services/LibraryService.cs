using System.Text.Json;
using MovieExplorer.Models;

namespace MovieExplorer.Services
{
    //this is to save/load favourites + history to a JSON file.
    public class LibraryService
    {
        private readonly string _filePath = Path.Combine(FileSystem.AppDataDirectory, "library.json");
        private LibraryData _data = new();
        //this is to get the favourites list for a page.
        public List<MovieMini> GetFavourites() => _data.Favourites;

        //this is to get the history list for a page.
        public List<MovieMini> GetHistory() => _data.History;

        public IReadOnlyList<MovieMini> GetWatchlist() => _data.Watchlist;

        // I call this once to load saved data (if the file exists).
        public async Task LoadAsync()
        {
            try
            {
                if (!File.Exists(_filePath))
                {
                    _data = new LibraryData();
                    return;
                }

                var json = await File.ReadAllTextAsync(_filePath);
                _data = JsonSerializer.Deserialize<LibraryData>(json) ?? new LibraryData();
            }
            catch
            {
                _data = new LibraryData();
            }
        }

        // I call this whenever I change favourites/history.
        private async Task SaveAsync()
        {
            var json = JsonSerializer.Serialize(_data, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(_filePath, json);
        }

        // I use this to check if a movie is already favourited.
        public bool IsFavourite(string? imdbId)
            => !string.IsNullOrWhiteSpace(imdbId) && _data.Favourites.Any(x => x.ImdbId == imdbId);

        // I use this to add/remove a movie from favourites.
        public async Task ToggleFavouriteAsync(Movie movie)
        {
            if (string.IsNullOrWhiteSpace(movie.ImdbId))
                return;

            var existing = _data.Favourites.FirstOrDefault(x => x.ImdbId == movie.ImdbId);

            if (existing != null)
                _data.Favourites.Remove(existing);
            else
                _data.Favourites.Insert(0, MovieMini.FromMovie(movie));

            await SaveAsync();
        }

        // I use this to log what I opened most recently in history.
        public async Task AddToHistoryAsync(Movie movie)
        {
            if (string.IsNullOrWhiteSpace(movie.ImdbId))
                return;

            _data.History.RemoveAll(x => x.ImdbId == movie.ImdbId);
            _data.History.Insert(0, MovieMini.FromMovie(movie));

            if (_data.History.Count > 50)
                LibraryData.TrimTo50(_data.History);

            await SaveAsync();
        }

        // This is the JSON structure I save to disk.
        private class LibraryData
        {
            public List<MovieMini> Favourites { get; set; } = new();
            public List<MovieMini> History { get; set; } = new();
            public List<MovieMini> Watchlist { get; set; } = new();

            public static void TrimTo50(List<MovieMini> list)
            {
                if (list.Count > 50)
                    list.RemoveRange(50, list.Count - 50);
            }
        }

        // This is a smaller movie object I store in the JSON file.
        public class MovieMini
        {
            public string ImdbId { get; set; } = "";
            public string Title { get; set; } = "";
            public string Year { get; set; } = "";
            public string Poster { get; set; } = "";

            public static MovieMini FromMovie(Movie m) => new()
            {
                ImdbId = m.ImdbId ?? "",
                Title = m.Title ?? "",
                Year = m.Year > 0 ? m.Year.ToString() : "",
                Poster = m.Poster ?? ""
            };

            public string PosterSafe => string.IsNullOrWhiteSpace(Poster) || Poster == "N/A"
                ? ""
                : Poster.Replace("http://", "https://");
        }

        // Clears all favourites and saves.
        public async Task ClearFavouritesAsync()
        {
            _data.Favourites.Clear();
            await SaveAsync();
        }

        // Clears all history and saves.
        public async Task ClearHistoryAsync()
        {
            _data.History.Clear();
            await SaveAsync();
        }

        public bool IsWatchlisted(string imdbId) => _data.Watchlist.Any(x => x.ImdbId == imdbId);

        public async Task ToggleWatchlistAsync(Movie movie)
        {
            var existing = _data.Watchlist.FirstOrDefault(x => x.ImdbId == movie.ImdbId);

            if (existing != null)
                _data.Watchlist.Remove(existing);
            else
                _data.Watchlist.Insert(0, MovieMini.FromMovie(movie));

            await SaveAsync();
        }

        public async Task ClearWatchlistAsync()
        {
            _data.Watchlist.Clear();
            await SaveAsync();
        }

        public async Task RemoveFavouriteAsync(string imdbId)
        {
            var item = _data.Favourites.FirstOrDefault(x => x.ImdbId == imdbId);
            if (item != null)
            {
                _data.Favourites.Remove(item);
                await SaveAsync();
            }
        }

        public async Task RemoveHistoryAsync(string imdbId)
        {
            var item = _data.History.FirstOrDefault(x => x.ImdbId == imdbId);
            if (item != null)
            {
                _data.History.Remove(item);
                await SaveAsync();
            }
        }

        public async Task RemoveWatchlistAsync(string imdbId)
        {
            var item = _data.Watchlist.FirstOrDefault(x => x.ImdbId == imdbId);
            if (item != null)
            {
                _data.Watchlist.Remove(item);
                await SaveAsync();
            }
        }

    }
}
