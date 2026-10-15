// ============================================================================
// PortfolioPage.xaml.cs  -  the CODE behind the Portfolio page
// ----------------------------------------------------------------------------
// This file:
//   1. creates the five filter buttons (All, Branding, Flyers, Posters, Social Media)
//   2. fills the gallery with the projects (from SiteContent.Projects)
//   3. re-fills it when you type in the search box or tap a filter
//   4. opens a project's detail page when a card is tapped
// ============================================================================
using AndiswaConsultationApp.Models;
using AndiswaConsultationApp.Services;
using Microsoft.Maui.Controls.Shapes;

namespace AndiswaConsultationApp.Pages;

public partial class PortfolioPage : ContentPage
{
    // The filter buttons: the first part is the "Filter" value of a project, the second is the text on the button
    private static readonly (string Key, string Text)[] Filters =
    {
        ("all", "All"),
        ("branding", "Branding"),
        ("flyers", "Flyers"),
        ("posters", "Posters"),
        ("social", "Social Media"),
    };

    private readonly Dictionary<string, Button> _filterButtons = new();   // so we can change their colours
    private string _filter = "all";                                       // the filter that is selected now

    public PortfolioPage()
    {
        InitializeComponent();
        BuildFilters();
        ApplyFilter();
    }

    // Creates one button for every filter
    private void BuildFilters()
    {
        foreach (var (key, text) in Filters)
        {
            var button = new Button
            {
                Text = text,
                FontSize = 14,
                FontAttributes = FontAttributes.Bold,
                CornerRadius = 20,
                HeightRequest = 40,
                Padding = new Thickness(20, 0),
                BorderWidth = 1.5
            };

            var chosen = key;     // remember which filter THIS button belongs to
            button.Clicked += (_, _) =>
            {
                _filter = chosen;
                UpdateFilterColours();
                ApplyFilter();
            };

            _filterButtons[key] = button;
            FilterLayout.Children.Add(button);
        }

        UpdateFilterColours();
    }

    // The selected button is navy with white text, the others are white with navy text
    private void UpdateFilterColours()
    {
        foreach (var (key, button) in _filterButtons)
        {
            var selected = key == _filter;
            button.BackgroundColor = selected ? Color.FromArgb("#1B2A41") : Colors.White;
            button.TextColor = selected ? Colors.White : Color.FromArgb("#1B2A41");
            button.BorderColor = selected ? Color.FromArgb("#1B2A41") : Color.FromArgb("#DDE2E8");
        }
    }

    // Works out which projects match the filter AND the search text, then shows them
    private void ApplyFilter()
    {
        var search = SearchEntry.Text?.Trim() ?? "";

        var matches = SiteContent.Projects
            .Where(p => _filter == "all" || p.Filter == _filter)
            .Where(p => search.Length == 0
                        || p.Title.Contains(search, StringComparison.OrdinalIgnoreCase)
                        || p.Category.Contains(search, StringComparison.OrdinalIgnoreCase)
                        || p.Blurb.Contains(search, StringComparison.OrdinalIgnoreCase))
            .ToList();

        CountLabel.Text = matches.Count == 1 ? "Showing 1 Project" : $"Showing {matches.Count} Projects";
        EmptyLabel.IsVisible = matches.Count == 0;
        BuildGallery(matches);
    }

    // Draws the cards, two per row
    private void BuildGallery(List<ProjectItem> projects)
    {
        Gallery.Children.Clear();
        Gallery.RowDefinitions.Clear();

        for (var i = 0; i < projects.Count; i++)
        {
            if (i % 2 == 0)                                       // a new row every two projects
                Gallery.RowDefinitions.Add(new RowDefinition(GridLength.Auto));

            Gallery.Add(BuildCard(projects[i]), i % 2, i / 2);    // column = 0 or 1, row = 0, 0, 1, 1 ...
        }
    }

    // One project card: picture on top, category + title + short text underneath
    private Border BuildCard(ProjectItem project)
    {
        var card = new Border
        {
            BackgroundColor = Colors.White,
            Stroke = Color.FromArgb("#DDE2E8"),
            StrokeThickness = 1,
            StrokeShape = new RoundRectangle { CornerRadius = 16 },
            Padding = 0,
            Content = new VerticalStackLayout
            {
                Spacing = 0,
                Children =
                {
                    new Image
                    {
                        Source = ImageSource.FromFile(project.Image),
                        Aspect = Aspect.AspectFit,           // show the whole design, never cropped
                        HeightRequest = 140,
                        BackgroundColor = Color.FromArgb("#EEF1F4")
                    },
                    new VerticalStackLayout
                    {
                        Padding = new Thickness(12, 10, 12, 14),
                        Spacing = 2,
                        Children =
                        {
                            new Label
                            {
                                Text = project.Category.ToUpperInvariant(),
                                FontSize = 11,
                                CharacterSpacing = 1,
                                FontAttributes = FontAttributes.Bold,
                                TextColor = Color.FromArgb("#3E6690")
                            },
                            new Label
                            {
                                Text = project.Title,
                                FontSize = 15,
                                FontAttributes = FontAttributes.Bold,
                                TextColor = Color.FromArgb("#1B2A41")
                            },
                            new Label
                            {
                                Text = project.Blurb,
                                FontSize = 12,
                                LineBreakMode = LineBreakMode.TailTruncation,
                                MaxLines = 2,
                                TextColor = Color.FromArgb("#5B6472")
                            }
                        }
                    }
                }
            }
        };

        // When the card is tapped, open the project's detail page
        var tap = new TapGestureRecognizer();
        tap.Tapped += async (_, _) => await AppNavigation.OpenProjectAsync(project);
        card.GestureRecognizers.Add(tap);

        return card;
    }

    // Runs every time a letter is typed in the search box
    private void OnSearchTextChanged(object? sender, TextChangedEventArgs e) => ApplyFilter();

    // "View Project" on the featured project
    private async void OnFeaturedClicked(object? sender, EventArgs e) =>
        await AppNavigation.OpenProjectAsync(SiteContent.FeaturedProject);
}
