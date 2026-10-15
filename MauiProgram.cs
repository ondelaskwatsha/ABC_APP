
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
        // The five tabs:
        builder.Services.AddTransient<HomePage>();
        builder.Services.AddTransient<ServicesPage>();
        builder.Services.AddTransient<PortfolioPage>();
        builder.Services.AddTransient<ContactPage>();
        builder.Services.AddTransient<MorePage>();

        // The pages that open on top of a tab (registered by name in AppShell.xaml.cs):
        builder.Services.AddTransient<AboutPage>();
        builder.Services.AddTransient<TeamPage>();
        builder.Services.AddTransient<TestimonialsPage>();
        builder.Services.AddTransient<ProjectDetailPage>();

#if DEBUG
        // While you are testing (Debug mode), show log messages in Visual Studio's Output window
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
