using MovieExplorer.Models;
using MovieExplorer.Services;

namespace MovieExplorer.Pages;

public partial class HistoryPage : ContentPage
{
    public HistoryPage()
    {
        InitializeComponent();
    }

    // Refreshes the list every time I open/return to this page.
    protected override void OnAppearing()
    {
        base.OnAppearing();
        RefreshHistory();
    }

    private void RefreshHistory()
    {
        HistoryView.ItemsSource = null;
        HistoryView.ItemsSource = App.Library.GetHistory();
    }

    private async void ClearHistory_Clicked(object sender, EventArgs e)
    {
        var ok = await DisplayAlert("Clear history?", "This will remove all history items.", "Clear", "Cancel");
        if (!ok) return;

        await App.Library.ClearHistoryAsync();
        RefreshHistory();
    }

    private async void HistoryView_SelectionChanged(object sender, SelectionChangedEventArgs e)
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

    private async void RemoveHistory_Invoked(object sender, EventArgs e)
    {
        var swipeItem = (SwipeItem)sender;
        var imdbId = swipeItem.CommandParameter as string;

        if (string.IsNullOrWhiteSpace(imdbId))
            return;

        await App.Library.RemoveHistoryAsync(imdbId);
        RefreshHistory();
    }
}