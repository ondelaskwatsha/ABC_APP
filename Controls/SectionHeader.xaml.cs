// DARK MODE: the default colours are theme colours (see the constructor), so the title
// follows dark / light mode. A page can still pick its own colours, like the navy sections do.
namespace AndiswaConsultationApp.Controls;

public partial class SectionHeader : ContentView
{
    public static readonly BindableProperty TagProperty =
        BindableProperty.Create(nameof(Tag), typeof(string), typeof(SectionHeader), string.Empty);

    public static readonly BindableProperty TitleMainProperty =
        BindableProperty.Create(nameof(TitleMain), typeof(string), typeof(SectionHeader), string.Empty);

    public static readonly BindableProperty TitleAccentProperty =
        BindableProperty.Create(nameof(TitleAccent), typeof(string), typeof(SectionHeader), string.Empty,
            propertyChanged: (b, _, _) => ((SectionHeader)b).OnPropertyChanged(nameof(HasAccent)));

    // Colours (these defaults are only a fallback: the constructor links them to the theme)
    public static readonly BindableProperty TagColorProperty =
        BindableProperty.Create(nameof(TagColor), typeof(Color), typeof(SectionHeader), Color.FromArgb("#3E6690"));

    public static readonly BindableProperty MainColorProperty =
        BindableProperty.Create(nameof(MainColor), typeof(Color), typeof(SectionHeader), Color.FromArgb("#1B2A41"));

    public static readonly BindableProperty AccentColorProperty =
        BindableProperty.Create(nameof(AccentColor), typeof(Color), typeof(SectionHeader), Color.FromArgb("#3E6690"));

    public string Tag
    {
        get => (string)GetValue(TagProperty);
        set => SetValue(TagProperty, value);
    }

    public string TitleMain
    {
        get => (string)GetValue(TitleMainProperty);
        set => SetValue(TitleMainProperty, value);
    }

    public string TitleAccent
    {
        get => (string)GetValue(TitleAccentProperty);
        set => SetValue(TitleAccentProperty, value);
    }

    public Color TagColor
    {
        get => (Color)GetValue(TagColorProperty);
        set => SetValue(TagColorProperty, value);
    }

    public Color MainColor
    {
        get => (Color)GetValue(MainColorProperty);
        set => SetValue(MainColorProperty, value);
    }

    public Color AccentColor
    {
        get => (Color)GetValue(AccentColorProperty);
        set => SetValue(AccentColorProperty, value);
    }

    public bool HasAccent => !string.IsNullOrWhiteSpace(TitleAccent);

    public SectionHeader()
    {
        InitializeComponent();

        // Link the default colours to the theme. If a page sets TagColor / MainColor / AccentColor
        // itself (for example on a navy section), that choice replaces these links.
        SetDynamicResource(TagColorProperty, "Steel");
        SetDynamicResource(MainColorProperty, "Navy");
        SetDynamicResource(AccentColorProperty, "Steel");
    }
}