using MovieExplorer.Models;

namespace MovieExplorer.Pages;

public partial class FiltersPage : ContentPage
{
    private readonly MovieFilters _filters;
    private readonly Action<MovieFilters> _onApply;

    public FiltersPage(MovieFilters current, Action<MovieFilters> onApply)
    {
        InitializeComponent();

        _filters = current;
        _onApply = onApply;

        TypePicker.ItemsSource = new List<string> { "Any", "Movie", "Series" };
        SortPicker.ItemsSource = new List<string> { "Title", "Year" };

        TypePicker.SelectedItem = _filters.Type;
        SortPicker.SelectedItem = _filters.Sort;

        YearFromEntry.Text = _filters.YearFrom?.ToString() ?? "";
        YearToEntry.Text = _filters.YearTo?.ToString() ?? "";
    }

    private async void Apply_Clicked(object sender, EventArgs e)
    {
        _filters.Type = (TypePicker.SelectedItem as string) ?? "Any";
        _filters.Sort = (SortPicker.SelectedItem as string) ?? "Title";

        _filters.YearFrom = int.TryParse(YearFromEntry.Text, out var yf) ? yf : null;
        _filters.YearTo = int.TryParse(YearToEntry.Text, out var yt) ? yt : null;

        _onApply(_filters);
        await Navigation.PopAsync();
    }

    private void Clear_Clicked(object sender, EventArgs e)
    {
        TypePicker.SelectedItem = "Any";
        SortPicker.SelectedItem = "Title";
        YearFromEntry.Text = "";
        YearToEntry.Text = "";
    }
}