using AndiswaConsultationApp.Services;

namespace AndiswaConsultationApp;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        // Loads the user's saved choices from Settings: dark / light mode and font style
        ThemeService.Initialize();
    }

    // Creates the app window and puts the AppShell in it
    protected override Window CreateWindow(IActivationState? activationState)
    {
        var window = new Window(new AppShell());

        if (DeviceInfo.Platform == DevicePlatform.WinUI)
        {
            window.Width = 420;
            window.Height = 860;
        }

        return window;
    }
}