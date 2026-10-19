using AndiswaConsultationApp.Pages;
using Windows.UI.ApplicationSettings;

namespace AndiswaConsultationApp;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();   // builds the tab bar described in AppShell.xaml

        // Pages that open on top of a tab (with a back arrow).
        Routing.RegisterRoute("about", typeof(AboutPage));
        Routing.RegisterRoute("team", typeof(TeamPage));
        Routing.RegisterRoute("testimonials", typeof(TestimonialsPage));
        Routing.RegisterRoute("projectdetail", typeof(ProjectDetailPage));
        Routing.RegisterRoute("settings", typeof(SettingsPage));
    }
}