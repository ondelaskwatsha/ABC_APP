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

    // "Settings": opens the Settings page on top of this one
    private async void OnSettingsTapped(object? sender, TappedEventArgs e) =>
        await AppNavigation.OpenPageAsync("settings");
}