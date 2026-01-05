using MovieExplorer.Models;
using MovieExplorer.Services;

namespace MovieExplorer.Pages;

public partial class FavouritesPage : ContentPage
{
    public FavouritesPage()
    {
        InitializeComponent();
    }

    // Refreshes the list every time I open/return to this page.
    protected override void OnAppearing()
    {
        base.OnAppearing();
        RefreshFavourites();
    }

    private void RefreshFavourites()
    {
        FavsView.ItemsSource = null;
        FavsView.ItemsSource = App.Library.GetFavourites();
    }

    private async void ClearFavourites_Clicked(object sender, EventArgs e)
    {
        var ok = await DisplayAlert("Clear favourites?",
                                   "This will remove all favourite movies.",
                                   "Clear", "Cancel");
        if (!ok) return;

        await App.Library.ClearFavouritesAsync();
        RefreshFavourites();
    }

    private async void FavsView_SelectionChanged(object sender, SelectionChangedEventArgs e)
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

    private async void RemoveFavourite_Invoked(object sender, EventArgs e)
    {
        var swipeItem = (SwipeItem)sender;
        var imdbId = swipeItem.CommandParameter as string;

        if (string.IsNullOrWhiteSpace(imdbId))
            return;

        await App.Library.RemoveFavouriteAsync(imdbId);
        RefreshFavourites();
    }
}