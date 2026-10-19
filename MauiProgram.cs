using AndiswaConsultationApp.Pages;
using AndiswaConsultationApp.Services;
using Microsoft.Extensions.Logging;

namespace AndiswaConsultationApp;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        // ConfigureFonts registers the fonts that Settings > Font style can choose.
        // The .ttf files must be in Resources/Fonts.
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSans");
                fonts.AddFont("Lora-Regular.ttf", "Lora");
                fonts.AddFont("RobotoMono-Regular.ttf", "RobotoMono");
            });

        builder.Services.AddSingleton<ApiService>();

        // The five tabs:
        builder.Services.AddTransient<HomePage>();
        builder.Services.AddTransient<ServicesPage>();
        builder.Services.AddTransient<PortfolioPage>();
        builder.Services.AddTransient<ContactPage>();
        builder.Services.AddTransient<MorePage>();

        // The pages that open on top of a tab:
        builder.Services.AddTransient<AboutPage>();
        builder.Services.AddTransient<TeamPage>();
        builder.Services.AddTransient<TestimonialsPage>();
        builder.Services.AddTransient<ProjectDetailPage>();
        builder.Services.AddTransient<SettingsPage>();


#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}