// ============================================================================
// BottomNavBar.cs  -  the navigation bar at the BOTTOM of the screen
// ----------------------------------------------------------------------------
// Why do we need our own bar?
//   .NET MAUI's built-in tab bar sits at the bottom on Android, but at the TOP
//   on Windows. To have the same bottom bar everywhere, AppShell.xaml hides the
//   built-in one (Shell.TabBarIsVisible="False") and every tab page shows this
//   bar instead, through the "TabPage" template in App.xaml.
//
// How it works:
//   - It draws 6 buttons: Home, Services, Portfolio, Contact, More, Account.
//   - Tapping one switches tab with AppNavigation.GoToTabAsync(...).
//   - The button of the tab you are on is shown in the main colour (Navy),
//     the others in the soft grey colour.
//   - The icons are drawn with Path (vector lines), so their colour can change
//     with dark / light mode. The path data is copied from the old tab_*.svg files.
// ============================================================================
using AndiswaConsultationApp.Services;
using Microsoft.Maui.Controls.Shapes;
using Path = Microsoft.Maui.Controls.Shapes.Path;

namespace AndiswaConsultationApp.Controls;

public class BottomNavBar : ContentView
{
    // One button of the bar: the tab route, the text under the icon, and the icon drawing
    private record NavItem(string Route, string Title, string IconData);

    private static readonly NavItem[] Items =
    {
        new("Home", "Home",
            "M3,9 L12,2 L21,9 V20 A2,2 0 0 1 19,22 H5 A2,2 0 0 1 3,20 Z M9,22 V12 H15 V22"),
        new("Services", "Services",
            "M16,20 V4 A2,2 0 0 0 14,2 H10 A2,2 0 0 0 8,4 V20 M4,6 H20 A2,2 0 0 1 22,8 V18 A2,2 0 0 1 20,20 H4 A2,2 0 0 1 2,18 V8 A2,2 0 0 1 4,6 Z"),
        new("Portfolio", "Portfolio",
            "M5,3 H19 A2,2 0 0 1 21,5 V19 A2,2 0 0 1 19,21 H5 A2,2 0 0 1 3,19 V5 A2,2 0 0 1 5,3 Z M9,7 A2,2 0 1 1 9,11 A2,2 0 1 1 9,7 Z M21,15 L17.9,11.9 A2,2 0 0 0 15.1,11.9 L6,21"),
        new("Contact", "Contact",
            "M4,4 H20 A2,2 0 0 1 22,6 V18 A2,2 0 0 1 20,20 H4 A2,2 0 0 1 2,18 V6 A2,2 0 0 1 4,4 Z M22,7 L13,12.7 A1.94,1.94 0 0 1 11,12.7 L2,7"),
        new("More", "More",
            "M4,3 H9 A1,1 0 0 1 10,4 V9 A1,1 0 0 1 9,10 H4 A1,1 0 0 1 3,9 V4 A1,1 0 0 1 4,3 Z " +
            "M15,3 H20 A1,1 0 0 1 21,4 V9 A1,1 0 0 1 20,10 H15 A1,1 0 0 1 14,9 V4 A1,1 0 0 1 15,3 Z " +
            "M15,14 H20 A1,1 0 0 1 21,15 V20 A1,1 0 0 1 20,21 H15 A1,1 0 0 1 14,20 V15 A1,1 0 0 1 15,14 Z " +
            "M4,14 H9 A1,1 0 0 1 10,15 V20 A1,1 0 0 1 9,21 H4 A1,1 0 0 1 3,20 V15 A1,1 0 0 1 4,14 Z"),
        // "Account" opens the client login, or the client dashboard if already logged in
        new("Account", "Account",
            "M19,21 V19 A4,4 0 0 0 15,15 H9 A4,4 0 0 0 5,19 V21 M12,3 A4,4 0 1 1 12,11 A4,4 0 1 1 12,3 Z"),
    };

    // The icon and text of each button, so we can recolour them when the tab changes
    private readonly List<(NavItem Item, Path Icon, Label Text)> _buttons = new();

    public BottomNavBar()
    {
        var grid = new Grid { Padding = new Thickness(4, 8, 4, 10) };

        for (int i = 0; i < Items.Length; i++)
        {
            var item = Items[i];
            grid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));   // equal width for every button

            var icon = new Path
            {
                Data = (Geometry)new PathGeometryConverter().ConvertFromInvariantString(item.IconData)!,
                StrokeThickness = 2,
                StrokeLineCap = PenLineCap.Round,
                StrokeLineJoin = PenLineJoin.Round,
                WidthRequest = 24,
                HeightRequest = 24,
                HorizontalOptions = LayoutOptions.Center
            };

            var text = new Label
            {
                Text = item.Title,
                FontSize = 11,
                HorizontalOptions = LayoutOptions.Center,
                LineBreakMode = LineBreakMode.NoWrap
            };

            var button = new VerticalStackLayout
            {
                Spacing = 4,
                Padding = new Thickness(0, 2),
                BackgroundColor = Colors.Transparent,   // so the whole area can be tapped, not only the icon
                Children = { icon, text }
            };

            var tap = new TapGestureRecognizer();
            tap.Tapped += async (_, _) => await OnItemTappedAsync(item);
            button.GestureRecognizers.Add(tap);

            SemanticProperties.SetDescription(button, item.Title);   // read out by screen readers

            grid.Add(button, i, 0);
            _buttons.Add((item, icon, text));
        }

        // The bar itself: a white (Surface) strip with a thin line on top
        var bar = new Border
        {
            StrokeThickness = 0,
            Content = new VerticalStackLayout
            {
                Spacing = 0,
                Children =
                {
                    new BoxView { HeightRequest = 1 }.Themed(BoxView.ColorProperty, "Line"),
                    grid
                }
            }
        }.Themed(VisualElement.BackgroundColorProperty, "Surface");

        Content = bar;

        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    private void OnLoaded(object? sender, EventArgs e)
    {
        // Recolour when the user moves to another tab, or when dark mode is switched on / off
        if (Shell.Current is not null)
            Shell.Current.Navigated += OnShellNavigated;
        ThemeService.Changed += Refresh;
        Refresh();
    }

    private void OnUnloaded(object? sender, EventArgs e)
    {
        if (Shell.Current is not null)
            Shell.Current.Navigated -= OnShellNavigated;
        ThemeService.Changed -= Refresh;
    }

    private void OnShellNavigated(object? sender, ShellNavigatedEventArgs e) => Refresh();

    // Works out which tab is open, from the address, for example "//Contact" -> "Contact"
    private static string CurrentTab()
    {
        var location = Shell.Current?.CurrentState?.Location?.OriginalString ?? "";
        var first = location.TrimStart('/').Split('/').FirstOrDefault() ?? "";

        // The login page and the dashboard both belong to the "Account" button
        return first is AppNavigation.ClientLoginRoute or AppNavigation.ClientDashboardRoute ? "Account" : first;
    }

    // Paints the open tab in Navy and the others in Soft grey
    private void Refresh()
    {
        var current = CurrentTab();
        foreach (var (item, icon, text) in _buttons)
        {
            var key = item.Route == current ? "Navy" : "Soft";
            icon.Themed(Shape.StrokeProperty, key);
            text.Themed(Label.TextColorProperty, key);
            text.FontAttributes = item.Route == current ? FontAttributes.Bold : FontAttributes.None;
        }
    }

    private static async Task OnItemTappedAsync(NavItem item)
    {
        // Tapping the tab you are already on does nothing
        if (item.Route == CurrentTab())
            return;

        if (item.Route == "Account")
            await AppNavigation.GoToAccountAsync();
        else
            await AppNavigation.GoToTabAsync(item.Route);
    }
}
