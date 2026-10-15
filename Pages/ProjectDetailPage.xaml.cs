// ============================================================================
// ProjectDetailPage.xaml.cs  -  the CODE behind the project detail page
// ----------------------------------------------------------------------------
// [QueryProperty(...)] means: "when someone opens this page with GoToAsync and passes a value
// called Project, put it in my Project property". AppNavigation.OpenProjectAsync does exactly that.
// ============================================================================
using AndiswaConsultationApp.Models;
using AndiswaConsultationApp.Services;

namespace AndiswaConsultationApp.Pages;

[QueryProperty(nameof(Project), "Project")]
public partial class ProjectDetailPage : ContentPage
{
    private ProjectItem? _project;

    // The project to show. When it arrives, show it on screen.
    public ProjectItem? Project
    {
        get => _project;
        set
        {
            _project = value;
            BindingContext = value;                // {Binding Title} etc. now read from this project
            if (value is not null)
                Title = value.Title;               // the title in the top bar
        }
    }

    public ProjectDetailPage()
    {
        InitializeComponent();
    }

    // "Request a Similar Design": go to the Contact form with this project's service already chosen
    private async void OnRequestClicked(object? sender, EventArgs e) =>
        await AppNavigation.RequestServiceAsync(_project?.ContactService ?? "Graphic Design");

    // "Go Back": go back one page (the same as the back arrow)
    private async void OnBackClicked(object? sender, EventArgs e) =>
        await AppNavigation.BackAsync();
}
