// ============================================================================
// SectionHeader.xaml.cs  -  the settings (properties) of the page title control
// ----------------------------------------------------------------------------
// A "BindableProperty" is a property you can set from XAML, for example
//   <controls:SectionHeader Tag="WHO WE ARE" ... />
// Each property below has two parts:
//   1. the BindableProperty (registers it with MAUI)
//   2. a normal C# property (get/set) that reads and writes it
// ============================================================================
namespace AndiswaConsultationApp.Controls;

public partial class SectionHeader : ContentView
{
    // The small capital label, for example "WHO WE ARE"
    public static readonly BindableProperty TagProperty =
        BindableProperty.Create(nameof(Tag), typeof(string), typeof(SectionHeader), string.Empty);

    // The dark part of the big title
    public static readonly BindableProperty TitleMainProperty =
        BindableProperty.Create(nameof(TitleMain), typeof(string), typeof(SectionHeader), string.Empty);

    // The light blue part of the big title.
    // propertyChanged: when it changes, tell the screen that HasAccent may have changed too.
    public static readonly BindableProperty TitleAccentProperty =
        BindableProperty.Create(nameof(TitleAccent), typeof(string), typeof(SectionHeader), string.Empty,
            propertyChanged: (b, _, _) => ((SectionHeader)b).OnPropertyChanged(nameof(HasAccent)));

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

    // True when there is a light blue part to show. The XAML uses it to hide an empty line.
    public bool HasAccent => !string.IsNullOrWhiteSpace(TitleAccent);

    public SectionHeader()
    {
        InitializeComponent();   // builds the layout from SectionHeader.xaml
    }
}
