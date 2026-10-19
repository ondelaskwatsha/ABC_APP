using AndiswaConsultationApp.Services;

namespace AndiswaConsultationApp.Pages;

public partial class SettingsPage : ContentPage
{
    // Each font row's tick mark, so we can move the tick when another font is chosen
    private readonly List<(string Family, Label Tick)> _ticks = new();

    public SettingsPage()
    {
        InitializeComponent();
        DarkSwitch.IsToggled = ThemeService.IsDark;   // show the saved choice
        BuildFontList();
    }

    private void OnDarkToggled(object? sender, ToggledEventArgs e)
    {
        if (e.Value != ThemeService.IsDark)
            ThemeService.SetDark(e.Value);
    }

    private void BuildFontList()
    {
        foreach (var (name, family) in ThemeService.Fonts)
        {
            var tick = new Label
            {
                Text = "?",
                FontSize = 20,
                FontAttributes = FontAttributes.Bold,
                VerticalOptions = LayoutOptions.Center,
                IsVisible = family == ThemeService.FontFamily
            }.Themed(Label.TextColorProperty, "Steel");
            _ticks.Add((family, tick));

            // Each name is written in its own font so the user can see it before choosing
            var texts = new VerticalStackLayout
            {
                Spacing = 2,
                VerticalOptions = LayoutOptions.Center,
                Children =
                {
                    new Label { Text = name, FontSize = 17, FontFamily = family }
                        .Themed(Label.TextColorProperty, "Navy"),
                    new Label { Text = "The quick brown fox jumps over the lazy dog", FontSize = 13, FontFamily = family }
                        .Themed(Label.TextColorProperty, "Muted")
                }
            };

            var row = new Grid
            {
                ColumnDefinitions =
                {
                    new ColumnDefinition(GridLength.Star),
                    new ColumnDefinition(GridLength.Auto)
                },
                ColumnSpacing = 10
            };
            row.Add(texts, 0, 0);
            row.Add(tick, 1, 0);

            var card = new Border
            {
                Style = (Style)Application.Current!.Resources["Card"],
                Padding = new Thickness(16, 12),
                Content = row
            };

            var chosen = family;
            var tap = new TapGestureRecognizer();
            tap.Tapped += (_, _) =>
            {
                ThemeService.SetFont(chosen);
                foreach (var (fam, t) in _ticks)
                    t.IsVisible = fam == chosen;
            };
            card.GestureRecognizers.Add(tap);

            FontList.Children.Add(card);
        }
    }
}