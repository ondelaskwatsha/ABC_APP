// ============================================================================
// AppShell.xaml.cs  -  the code behind the navigation
// ----------------------------------------------------------------------------
// The tabs are in AppShell.xaml. Here we register the pages that are NOT tabs,
// so that GoToAsync("about") (etc.) knows which page to open.
// TEAM NOTE: if you add a new page that opens on top of another, register it here
//            AND in MauiProgram.cs.
// ============================================================================
using AndiswaConsultationApp.Pages;

namespace AndiswaConsultationApp;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();   // builds the tab bar described in AppShell.xaml

        // Pages that open on top of a tab (with a back arrow). The name is what you pass to GoToAsync.
        Routing.RegisterRoute("about", typeof(AboutPage));
        Routing.RegisterRoute("team", typeof(TeamPage));
        Routing.RegisterRoute("testimonials", typeof(TestimonialsPage));
        Routing.RegisterRoute("projectdetail", typeof(ProjectDetailPage));
    }
}
