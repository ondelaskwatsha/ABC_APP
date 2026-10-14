// ============================================================================
// MauiProgram.cs  -  where the APP STARTS
// ----------------------------------------------------------------------------
// When the app opens (on Android or Windows), .NET MAUI calls CreateMauiApp().
// Here we "register" everything the app needs, so that pages can simply ask
// for it in their constructor. This is called "dependency injection":
//   HomePage needs an ApiService -> MAUI creates/gives it automatically.
// ============================================================================
using AndiswaConsultationApp.Pages;
using AndiswaConsultationApp.Services;
using Microsoft.Extensions.Logging;

namespace AndiswaConsultationApp;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        // Use our App class (App.xaml) as the starting point
        builder.UseMauiApp<App>();

        // AddSingleton = ONE ApiService for the whole app, shared by every page.
        // That way Home and Contact share the data they downloaded.
        builder.Services.AddSingleton<ApiService>();

        // AddTransient = a fresh page is created when it is needed.
        // TEAM NOTE: every page must be registered here. If you add a new page, add a line.
        builder.Services.AddTransient<HomePage>();
        builder.Services.AddTransient<AboutPage>();
        builder.Services.AddTransient<ServicesPage>();
        builder.Services.AddTransient<PortfolioPage>();
        builder.Services.AddTransient<ContactPage>();

#if DEBUG
        // While you are testing (Debug mode), show log messages in Visual Studio's Output window
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
