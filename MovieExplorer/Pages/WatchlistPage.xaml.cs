using MovieExplorer.Models;
using MovieExplorer.Services;

namespace MovieExplorer.Pages;

public partial class WatchlistPage : ContentPage
{
    public WatchlistPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        Refresh();
    }

    private void Refresh()
    {
        WatchlistView.ItemsSource = null;
        WatchlistView.ItemsSource = App.Library.GetWatchlist();
    }

    private async void ClearWatchlist_Clicked(object sender, EventArgs e)
    {
        var ok = await DisplayAlert("Clear watchlist?",
            "This will remove all watchlist movies.", "Clear", "Cancel");
        if (!ok) return;

        await App.Library.ClearWatchlistAsync();
        Refresh();
    }

    private async void WatchlistView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var item = e.CurrentSelection.FirstOrDefault() as LibraryService.MovieMini;
        if (item == null) return;

        ((CollectionView)sender).SelectedItem = null;

        var movie = new Movie
        {
            ImdbId = item.ImdbId,
            Title = item.Title,
            Poster = item.Poster,
            Year = int.TryParse(item.Year, out var y) ? y : 0
        };

        await Navigation.PushAsync(new MovieDetailsPage(movie));
    }

    private async void RemoveWatchlist_Invoked(object sender, EventArgs e)
    {
        var swipeItem = (SwipeItem)sender;
        var imdbId = swipeItem.CommandParameter as string;

        if (string.IsNullOrWhiteSpace(imdbId))
            return;

        await App.Library.RemoveWatchlistAsync(imdbId);
        Refresh();
    }
}