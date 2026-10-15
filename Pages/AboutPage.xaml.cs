// ============================================================================
// AboutPage.xaml.cs  -  the CODE behind the About page
// ----------------------------------------------------------------------------
// The two "Who we are" paragraphs come from the website's settings (through the API), like on the Home page.
// Everything else on this page is fixed website text that lives in SiteContent (Models/Models.cs).
// ============================================================================
using AndiswaConsultationApp.Services;

namespace AndiswaConsultationApp.Pages;

public partial class AboutPage : ContentPage
{
    private readonly ApiService _api;   // our link to the API (given to us by MauiProgram.cs)

    public AboutPage(ApiService api)
    {
        InitializeComponent();
        _api = api;
    }

    // Runs every time the page comes on screen
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        try
        {
            // Show the saved copy straight away (never uses the internet), so the page is never empty
            if (BindingContext is null)
                BindingContext = await _api.GetSavedStudioAsync();

            // Then ask for the latest. Offline? ApiService quietly gives us the saved copy again.
            BindingContext = await _api.GetStudioAsync();
        }
        catch
        {
            // The text on this page is not important enough to show an error box: keep what we have.
        }
    }

    // "Meet the Team": opens the Team page on top of this one
    private async void OnTeamClicked(object? sender, EventArgs e) =>
        await AppNavigation.OpenPageAsync("team");
}
