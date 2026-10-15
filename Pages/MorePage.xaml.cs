// ============================================================================
// MorePage.xaml.cs  -  the CODE behind the "More" tab
// ----------------------------------------------------------------------------
// Tapping a row opens that page on top of this one (with a back arrow).
// ============================================================================
using AndiswaConsultationApp.Models;
using AndiswaConsultationApp.Services;

namespace AndiswaConsultationApp.Pages;

public partial class MorePage : ContentPage
{
    public MorePage()
    {
        InitializeComponent();
    }

    // "sender" is the row; its BindingContext is the MenuLink that row shows
    private async void OnLinkTapped(object? sender, TappedEventArgs e)
    {
        if (sender is BindableObject row && row.BindingContext is MenuLink link)
            await AppNavigation.OpenPageAsync(link.Route);
    }
}
