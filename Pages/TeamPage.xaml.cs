// ============================================================================
// TeamPage.xaml.cs  -  the CODE behind the Team page
// ----------------------------------------------------------------------------
// Only one thing to do: tapping a person's email opens the mail app.
// ============================================================================
using AndiswaConsultationApp.Models;

namespace AndiswaConsultationApp.Pages;

public partial class TeamPage : ContentPage
{
    public TeamPage()
    {
        InitializeComponent();
    }

    // "sender" is the email label; its BindingContext is the TeamMember that card shows
    private async void OnEmailTapped(object? sender, TappedEventArgs e)
    {
        if (sender is BindableObject label && label.BindingContext is TeamMember member)
        {
            try { await Launcher.OpenAsync($"mailto:{member.Email}"); } catch { }
        }
    }
}
