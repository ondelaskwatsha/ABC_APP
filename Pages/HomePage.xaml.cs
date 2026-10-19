using AndiswaConsultationApp.Models;
using AndiswaConsultationApp.Services;

namespace AndiswaConsultationApp.Pages;

public partial class HomePage : ContentPage
{
    private readonly ApiService _api;   // our link to the API (given to us by MauiProgram.cs)
    private bool _loaded;               // true once we got data from the SERVER (not just the saved copy)

    public HomePage(ApiService api)
    {
        InitializeComponent();
        _api = api;

        // What happens when the user pulls the page down
        Refresh.Command = new Command(async () =>
        {
            await LoadAsync(forceRefresh: true);
            Refresh.IsRefreshing = false;
        });

        // What happens when the user taps "Try again" in the red box
        ErrorBox.Retry += async (_, _) => await LoadAsync(forceRefresh: true);
    }

    // Runs every time the page comes on screen
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (!_loaded)
            await LoadAsync(forceRefresh: false);
    }

    private async Task LoadAsync(bool forceRefresh)
    {
        ErrorBox.Hide();

        try
        {
            // Step 1: show the saved copy straight away, so the page is never empty
            if (BindingContext is null)
                Show(await _api.GetSavedStudioAsync());

            // Step 2: ask the server for the latest information
            var studio = await _api.GetStudioAsync(forceRefresh);
            Show(studio);

            // Step 3: if we are offline, tell the user and try the server again next time
            _loaded = !_api.UsingOfflineData;
            if (_api.UsingOfflineData)
                ErrorBox.ShowInfo("You're offline. Showing the information saved on your phone.");
        }
        catch (Exception ex)
        {
            ErrorBox.Show(ApiService.FriendlyError(ex));
        }
    }

    private void Show(StudioInfo studio)
    {
        BindingContext = studio;
        BuildStats(studio.Stats);
        Footer.ApplyStudio(studio);
    }

    // Draws the 2 x 2 grid of numbers (built in code because they come from the database).
    // ".Themed(...)" ties a colour to the theme so it follows dark / light mode.
    private void BuildStats(List<StatItem> stats)
    {
        StatsGrid.Children.Clear();
        StatsGrid.RowDefinitions.Clear();

        for (var i = 0; i < stats.Count; i++)
        {
            if (i % 2 == 0)
                StatsGrid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));

            var card = new Border
            {
                StrokeThickness = 0.5,
                Padding = new Thickness(0, 30),
                Content = new VerticalStackLayout
                {
                    Spacing = 6,
                    HorizontalOptions = LayoutOptions.Center,
                    Children =
                    {
                        new Label
                        {
                            Text = stats[i].Value,
                            FontSize = 40,
                            FontAttributes = FontAttributes.Bold,
                            HorizontalOptions = LayoutOptions.Center
                        }.Themed(Label.TextColorProperty, "Navy"),
                        new Label
                        {
                            Text = stats[i].Label,
                            FontSize = 13,
                            HorizontalOptions = LayoutOptions.Center
                        }.Themed(Label.TextColorProperty, "Muted")
                    }
                }
            }
            .Themed(Border.StrokeProperty, "Line")
            .Themed(VisualElement.BackgroundColorProperty, "Surface");

            StatsGrid.Add(card, i % 2, i / 2);
        }
    }

    private async void OnViewPortfolioClicked(object? sender, EventArgs e) =>
        await AppNavigation.GoToTabAsync("Portfolio");

    private async void OnQuoteClicked(object? sender, EventArgs e) =>
        await AppNavigation.GoToTabAsync("Contact");

    private async void OnAboutClicked(object? sender, EventArgs e) =>
        await AppNavigation.OpenPageAsync("about");

    private async void OnServicesClicked(object? sender, EventArgs e) =>
        await AppNavigation.GoToTabAsync("Services");

    private async void OnFeaturedClicked(object? sender, EventArgs e) =>
        await AppNavigation.OpenProjectAsync(SiteContent.FeaturedProject);

    private async void OnTestimonialsClicked(object? sender, EventArgs e) =>
        await AppNavigation.OpenPageAsync("testimonials");
}