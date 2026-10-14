// ============================================================================
// HomePage.xaml.cs  -  the CODE behind the Home page
// ----------------------------------------------------------------------------
// HomePage.xaml is the layout. This file:
//   1. loads the studio information (from the API, or the saved copy when offline)
//   2. puts it on the screen (BindingContext + the four number boxes)
//   3. handles the two buttons and pull-to-refresh
// ============================================================================
using AndiswaConsultationApp.Models;
using AndiswaConsultationApp.Services;

namespace AndiswaConsultationApp.Pages;

public partial class HomePage : ContentPage
{
    private readonly ApiService _api;   // our link to the API (given to us by MauiProgram.cs)
    private bool _loaded;               // true once we got data from the SERVER (not just the saved copy)

    // The constructor runs once when the page is created.
    // "ApiService api" is handed to us automatically because we registered it in MauiProgram.cs.
    public HomePage(ApiService api)
    {
        InitializeComponent();          // builds the layout from HomePage.xaml
        _api = api;

        // What happens when the user pulls the page down
        Refresh.Command = new Command(async () =>
        {
            await LoadAsync(forceRefresh: true);
            Refresh.IsRefreshing = false;   // stop the spinning circle
        });

        // What happens when the user taps "Try again" in the red box
        ErrorBox.Retry += async (_, _) => await LoadAsync(forceRefresh: true);
    }

    // OnAppearing runs every time the page comes on screen (for example when you tap the Home tab)
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (!_loaded)                   // only load if we don't have server data yet
            await LoadAsync(forceRefresh: false);
    }

    // Loads the data and shows it
    private async Task LoadAsync(bool forceRefresh)
    {
        ErrorBox.Hide();

        try
        {
            // Step 1: show the saved copy straight away, so the page is never empty
            //         while we wait for the server (BindingContext is null = nothing shown yet)
            if (BindingContext is null)
                Show(await _api.GetSavedStudioAsync());

            // Step 2: ask the server for the latest information
            //         (if there is no internet, ApiService quietly gives us the saved copy)
            var studio = await _api.GetStudioAsync(forceRefresh);
            Show(studio);

            // Step 3: if we are offline, tell the user and try the server again next time
            _loaded = !_api.UsingOfflineData;
            if (_api.UsingOfflineData)
                ErrorBox.ShowInfo("You're offline. Showing the information saved on your phone.");
        }
        catch (Exception ex)
        {
            // Something unexpected went wrong: show the red box
            ErrorBox.Show(ApiService.FriendlyError(ex));
        }
    }

    // Puts the data on screen
    private void Show(StudioInfo studio)
    {
        // BindingContext = "the data the XAML {Binding ...} reads from".
        // After this line, {Binding HeroText} shows studio.HeroText, and so on.
        BindingContext = studio;
        BuildStats(studio.Stats);
    }

    // Draws the 2 x 2 grid of numbers. We build it in code because the numbers
    // come from the database and there could be more or fewer than four.
    private void BuildStats(List<StatItem> stats)
    {
        // Remove the old boxes first (for example after pull-to-refresh)
        StatsGrid.Children.Clear();
        StatsGrid.RowDefinitions.Clear();

        for (var i = 0; i < stats.Count; i++)
        {
            // Every two numbers we need a new row (i = 0, 2, 4 ...)
            if (i % 2 == 0)
                StatsGrid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));

            // One box: big blue number with a small label underneath
            var card = new Border
            {
                Stroke = Color.FromArgb("#CFE6F7"),
                StrokeThickness = 0.5,
                BackgroundColor = Colors.White,
                Padding = new Thickness(0, 30),
                Content = new VerticalStackLayout
                {
                    Spacing = 6,
                    HorizontalOptions = LayoutOptions.Center,
                    Children =
                    {
                        new Label
                        {
                            Text = stats[i].Value,                    // for example "50+"
                            FontSize = 40,
                            FontAttributes = FontAttributes.Bold,
                            TextColor = Color.FromArgb("#1F7AC2"),
                            HorizontalOptions = LayoutOptions.Center
                        },
                        new Label
                        {
                            Text = stats[i].Label,                    // for example "Projects Completed"
                            FontSize = 13,
                            TextColor = Color.FromArgb("#0F2A3D"),
                            HorizontalOptions = LayoutOptions.Center
                        }
                    }
                }
            };

            // Put the box in the grid: column = i % 2 (0 or 1), row = i / 2 (0, 0, 1, 1 ...)
            StatsGrid.Add(card, i % 2, i / 2);
        }
    }

    // "View Portfolio" button: jump to the Portfolio tab. "//" means "go to that tab".
    private async void OnViewPortfolioClicked(object? sender, EventArgs e) =>
        await Shell.Current.GoToAsync("//Portfolio");

    // "Get a Free Quote" button: jump to the Contact tab
    private async void OnQuoteClicked(object? sender, EventArgs e) =>
        await Shell.Current.GoToAsync("//Contact");
}
