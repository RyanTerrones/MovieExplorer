using System;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.Maui.Controls;

namespace MovieExplorer.Pages
{
    public partial class Top250Page : ContentPage
    {
        public ObservableCollection<TopItem> Items { get; } = new();

        public Top250Page()
        {
            InitializeComponent();
            BindingContext = this;

            // load once at startup
            LoadTop25();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (Items.Count == 0)
                LoadTop25();
        }

        private void LoadTop25()
        {
            Items.Clear();

            var top25 = new[]
            {
                new TopItem("The Shawshank Redemption", "Rank #1 • 1994"),
                new TopItem("The Godfather", "Rank #2 • 1972"),
                new TopItem("The Dark Knight", "Rank #3 • 2008"),
                new TopItem("The Godfather Part II", "Rank #4 • 1974"),
                new TopItem("12 Angry Men", "Rank #5 • 1957"),
                new TopItem("Schindler's List", "Rank #6 • 1993"),
                new TopItem("The Lord of the Rings: The Return of the King", "Rank #7 • 2003"),
                new TopItem("Pulp Fiction", "Rank #8 • 1994"),
                new TopItem("The Lord of the Rings: The Fellowship of the Ring", "Rank #9 • 2001"),
                new TopItem("The Good, the Bad and the Ugly", "Rank #10 • 1966"),
                new TopItem("Forrest Gump", "Rank #11 • 1994"),
                new TopItem("Fight Club", "Rank #12 • 1999"),
                new TopItem("Inception", "Rank #13 • 2010"),
                new TopItem("The Lord of the Rings: The Two Towers", "Rank #14 • 2002"),
                new TopItem("Star Wars: Episode V - The Empire Strikes Back", "Rank #15 • 1980"),
                new TopItem("The Matrix", "Rank #16 • 1999"),
                new TopItem("Goodfellas", "Rank #17 • 1990"),
                new TopItem("One Flew Over the Cuckoo's Nest", "Rank #18 • 1975"),
                new TopItem("Seven Samurai", "Rank #19 • 1954"),
                new TopItem("Se7en", "Rank #20 • 1995"),
                new TopItem("Interstellar", "Rank #21 • 2014"),
                new TopItem("The Silence of the Lambs", "Rank #22 • 1991"),
                new TopItem("Saving Private Ryan", "Rank #23 • 1998"),
                new TopItem("Spirited Away", "Rank #24 • 2001"),
                new TopItem("The Green Mile", "Rank #25 • 1999"),
            };

            foreach (var item in top25)
                Items.Add(item);
        }

        private async void List_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.FirstOrDefault() is not TopItem item)
                return;

            // unselect
            if (sender is CollectionView cv)
                cv.SelectedItem = null;

            await DisplayAlert("Selected", $"{item.Title}\n{item.Subtitle}", "OK");
        }

        private void Menu_Clicked(object sender, EventArgs e)
        {
            MainFlyoutPage.Current?.ToggleFlyout();
        }

        private async void Back_Clicked(object sender, EventArgs e)
        {
            // prefer pop if it exists
            if (Navigation?.NavigationStack?.Count > 1)
            {
                await Navigation.PopAsync();
                return;
            }

            // otherwise go home
            MainFlyoutPage.Current?.NavigateTo(new MoviesPage());
        }

        public record TopItem(string Title, string Subtitle);
    }
}
