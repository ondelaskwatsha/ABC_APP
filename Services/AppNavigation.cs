// ============================================================================
// AppNavigation.cs  -  ONE place for "go to that page" code
// ----------------------------------------------------------------------------
// Every page uses these helpers, so navigation always works the same way.
//
//   "//Contact"          = SWITCH to the Contact tab ("//" means "start from the tab bar")
//   "about", "team" ...  = OPEN that page on top of the current one (it gets a back arrow)
//
// The page names ("about", "team", "testimonials", "projectdetail") are registered in AppShell.xaml.cs.
// ============================================================================
using AndiswaConsultationApp.Models;

namespace AndiswaConsultationApp.Services;

public static class AppNavigation
{
    // The three Account pages (login, register, dashboard). They are tabs too (see AppShell.xaml), but the bottom bar
    // shows them as ONE button called "Account".
    public const string ClientLoginRoute = "ClientLogin";
    public const string ClientDashboardRoute = "ClientDashboard";
    public const string ClientRegisterRoute = "ClientRegister";

    // Switch to one of the tabs: "Home", "Services", "Portfolio", "Contact" or "More"
    public static Task GoToTabAsync(string tab) =>
        Shell.Current.GoToAsync($"//{tab}");

    // The "Account" button: the dashboard if the client is logged in, otherwise the login page
    public static async Task GoToAccountAsync()
    {
        await ClientSession.LoadAsync();   // reads the saved login the first time
        await GoToTabAsync(ClientSession.IsLoggedIn ? ClientDashboardRoute : ClientLoginRoute);
    }

    // Open a page that is not a tab: "about", "team" or "testimonials"
    public static Task OpenPageAsync(string route) =>
        Shell.Current.GoToAsync(route);

    // Open the detail page of one project (it receives the project through the "Project" key)
    public static Task OpenProjectAsync(ProjectItem project) =>
        Shell.Current.GoToAsync("projectdetail", new Dictionary<string, object> { ["Project"] = project });

    // Go to the Contact tab with a service already chosen in the drop-down
    public static Task RequestServiceAsync(string? contactService)
    {
        AppState.PendingService = contactService;
        return GoToTabAsync("Contact");
    }

    // Go back one page (the same as the back arrow)
    public static Task BackAsync() =>
        Shell.Current.GoToAsync("..");
}
