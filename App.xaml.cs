
namespace AndiswaConsultationApp;

public partial class App : Application
{
    public App()
    {
        // Loads App.xaml (the colours and styles)
        InitializeComponent();

        // The design is light blue and white
        
        UserAppTheme = AppTheme.Light;
    }

    // Creates the app window and puts the AppShell  in it
    protected override Window CreateWindow(IActivationState? activationState)
    {
        var window = new Window(new AppShell());

        
        // WinUI is Microsoft's name for Windows apps. On Android this part is skipped.
        if (DeviceInfo.Platform == DevicePlatform.WinUI)
        {
            window.Width = 420;
            window.Height = 860;
        }

        return window;
    }
}
