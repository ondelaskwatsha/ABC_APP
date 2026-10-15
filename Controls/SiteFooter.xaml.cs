// ============================================================================
// SiteFooter.xaml.cs  -  the code behind the footer
// ----------------------------------------------------------------------------
// Builds the five social buttons (Facebook, Instagram, X, LinkedIn, YouTube) like the website footer.
// If the database has a link for a network, tapping opens it. If not, a friendly note is shown,
// because the website's own social buttons do not have links yet either.
// ============================================================================
using AndiswaConsultationApp.Models;
using Microsoft.Maui.Controls.Shapes;

namespace AndiswaConsultationApp.Controls;

public partial class SiteFooter : ContentView
{
    public SiteFooter()
    {
        InitializeComponent();
        ApplyStudio(AppState.Studio);      // use the links we already know (if any)
    }

    /// <summary>Call this when fresh studio information arrives, so the social links are up to date.</summary>
    public void ApplyStudio(StudioInfo? studio)
    {
        SocialLayout.Children.Clear();

        AddSocial("f", "Facebook", studio?.FacebookUrl);
        AddSocial("IG", "Instagram", studio?.InstagramUrl);
        AddSocial("X", "X (Twitter)", studio?.TwitterUrl);
        AddSocial("in", "LinkedIn", studio?.LinkedInUrl);
        AddSocial("YT", "YouTube", studio?.YouTubeUrl);
    }

    private void AddSocial(string text, string network, string? url)
    {
        // A 44 x 44 box with corners of 22 = a circle
        var circle = new Border
        {
            BackgroundColor = Color.FromArgb("#24384F"),
            StrokeThickness = 0,
            StrokeShape = new RoundRectangle { CornerRadius = 22 },
            WidthRequest = 44,
            HeightRequest = 44,
            Content = new Label
            {
                Text = text,
                FontSize = 14,
                FontAttributes = FontAttributes.Bold,
                TextColor = Color.FromArgb("#F7F8FA"),
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            }
        };

        var tap = new TapGestureRecognizer();
        tap.Tapped += async (_, _) =>
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                if (Shell.Current is not null)
                    await Shell.Current.DisplayAlert(network, $"Our {network} page is coming soon.", "OK");
                return;
            }

            try { await Launcher.OpenAsync(new Uri(url)); } catch { }
        };
        circle.GestureRecognizers.Add(tap);

        SocialLayout.Children.Add(circle);
    }

    // Tapping the phone number opens the dialler (spaces removed first)
    private async void OnPhoneTapped(object? sender, TappedEventArgs e)
    {
        try { await Launcher.OpenAsync($"tel:{SiteContent.Phone.Replace(" ", "")}"); } catch { }
    }

    // Tapping the email opens the mail app with the address filled in
    private async void OnEmailTapped(object? sender, TappedEventArgs e)
    {
        try { await Launcher.OpenAsync($"mailto:{SiteContent.Email}"); } catch { }
    }
}
