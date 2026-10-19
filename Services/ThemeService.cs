namespace AndiswaConsultationApp.Services;

public static class ThemeService
{
    private const string DarkKey = "dark_mode";
    private const string FontKey = "font_family";

    /// <summary>Raised after a theme or font change.</summary>
    public static event Action? Changed;

    // Name shown on the Settings page, and the alias registered in MauiProgram.cs. "" = phone's normal font.
    public static readonly (string Name, string Family)[] Fonts =
    {
        ("System default", ""),
        ("Open Sans", "OpenSans"),
        ("Lora (serif)", "Lora"),
        ("Roboto Mono", "RobotoMono"),
    };

    private static readonly Dictionary<string, string> Light = new()
    {
        ["Navy"] = "#1B2A41",
        ["NavyDeep"] = "#101B2B",
        ["NavySoft"] = "#24384F",
        ["Steel"] = "#3E6690",
        ["SteelLight"] = "#5B8DB8",
        ["Cream"] = "#F7F8FA",
        ["Mist"] = "#EEF1F4",
        ["Ink"] = "#232830",
        ["Muted"] = "#5B6472",
        ["Soft"] = "#8891A0",
        ["Line"] = "#DDE2E8",
        ["OnDarkMuted"] = "#B9C4D2",
        ["PageBg"] = "#F7F8FA",
        ["Surface"] = "#FFFFFF",
        ["ErrorBg"] = "#FFF4F4",
        ["ErrorLine"] = "#E0B4B4",
        ["ErrorText"] = "#8A1F1F",
    };

    private static readonly Dictionary<string, string> Dark = new()
    {
        ["Navy"] = "#5A8BBD",
        ["NavyDeep"] = "#0A1018",
        ["NavySoft"] = "#4A78A8",
        ["Steel"] = "#8DB4DB",
        ["SteelLight"] = "#9CC0E3",
        ["Cream"] = "#F7F8FA",
        ["Mist"] = "#141E2C",   // Cream stays light: it is used as text on dark sections
        ["Ink"] = "#E6EBF2",
        ["Muted"] = "#A7B2C0",
        ["Soft"] = "#7C8797",
        ["Line"] = "#2C3A4E",
        ["OnDarkMuted"] = "#E3EAF3",
        ["PageBg"] = "#0E1621",
        ["Surface"] = "#1A2535",
        ["ErrorBg"] = "#3A1F24",
        ["ErrorLine"] = "#7A3B43",
        ["ErrorText"] = "#F2B8B8",
    };

    public static bool IsDark => Preferences.Default.Get(DarkKey, false);
    public static string FontFamily => Preferences.Default.Get(FontKey, "");

    /// <summary>Call once when the app starts (App.xaml.cs).</summary>
    public static void Initialize() => Apply();

    public static void SetDark(bool dark)
    {
        Preferences.Default.Set(DarkKey, dark);
        Apply();
    }

    public static void SetFont(string family)
    {
        Preferences.Default.Set(FontKey, family);
        Apply();
    }

    private static void Apply()
    {
        var app = Application.Current;
        if (app is null) return;

        var resources = app.Resources;
        foreach (var (key, hex) in IsDark ? Dark : Light)
            resources[key] = Color.FromArgb(hex);

        resources["AppFontFamily"] = FontFamily;
        app.UserAppTheme = IsDark ? AppTheme.Dark : AppTheme.Light;   // also darkens system pop-ups

        Changed?.Invoke();
    }

    /// <summary>Reads the CURRENT value of a theme colour.</summary>
    public static Color Get(string key) =>
        Application.Current!.Resources.TryGetValue(key, out var value) && value is Color c ? c : Colors.Magenta;

    /// <summary>
    /// For views built in C#: ties a property to a theme colour so it follows dark / light mode.
    /// Example: new Label { Text = "Hi" }.Themed(Label.TextColorProperty, "Navy")
    /// </summary>
    public static T Themed<T>(this T view, BindableProperty property, string resourceKey) where T : Element
    {
        view.SetDynamicResource(property, resourceKey);
        return view;
    }
}