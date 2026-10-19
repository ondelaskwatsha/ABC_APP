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

    // While this page is on screen, re-colour the filter buttons if dark mode is switched
    protected override void OnAppearing()
    {
        base.OnAppearing();
        ThemeService.Changed += UpdateFilterColours;
        UpdateFilterColours();      // also catches a change made while the page was away
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        ThemeService.Changed -= UpdateFilterColours;
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

    // The selected button is navy with white text, the others use the card colour with navy text.
    // The colours are read from the current theme, so they work in dark mode too.
    private void UpdateFilterColours()
    {
        var navy = ThemeService.Get("Navy");
        var surface = ThemeService.Get("Surface");
        var line = ThemeService.Get("Line");

        foreach (var (key, button) in _filterButtons)
        {
            var selected = key == _filter;
            button.BackgroundColor = selected ? navy : surface;
            button.TextColor = selected ? Colors.White : navy;
            button.BorderColor = selected ? navy : line;
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
            if (i % 2 == 0)
                Gallery.RowDefinitions.Add(new RowDefinition(GridLength.Auto));

            Gallery.Add(BuildCard(projects[i]), i % 2, i / 2);
        }
    }

    // One project card. ".Themed(...)" ties a colour to the theme so it follows dark / light mode.
    private Border BuildCard(ProjectItem project)
    {
        var card = new Border
        {
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
                        HeightRequest = 140
                    }.Themed(VisualElement.BackgroundColorProperty, "Mist"),
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
                                FontAttributes = FontAttributes.Bold
                            }.Themed(Label.TextColorProperty, "Steel"),
                            new Label
                            {
                                Text = project.Title,
                                FontSize = 15,
                                FontAttributes = FontAttributes.Bold
                            }.Themed(Label.TextColorProperty, "Navy"),
                            new Label
                            {
                                Text = project.Blurb,
                                FontSize = 12,
                                LineBreakMode = LineBreakMode.TailTruncation,
                                MaxLines = 2
                            }.Themed(Label.TextColorProperty, "Muted")
                        }
                    }
                }
            }
        }
        .Themed(Border.StrokeProperty, "Line")
        .Themed(VisualElement.BackgroundColorProperty, "Surface");

        var tap = new TapGestureRecognizer();
        tap.Tapped += async (_, _) => await AppNavigation.OpenProjectAsync(project);
        card.GestureRecognizers.Add(tap);

        return card;
    }

    private void OnSearchTextChanged(object? sender, TextChangedEventArgs e) => ApplyFilter();

    private async void OnFeaturedClicked(object? sender, EventArgs e) =>
        await AppNavigation.OpenProjectAsync(SiteContent.FeaturedProject);
}