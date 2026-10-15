// ============================================================================
// ServicesPage.xaml.cs  -  the CODE behind the Services page
// ----------------------------------------------------------------------------
// The layout and the words are in ServicesPage.xaml / SiteContent.
// This file only handles the tap on a service card:
//   it remembers which service was tapped, then opens the Contact tab,
//   where the form's "Service Required" drop-down is already filled in.
// ============================================================================
using AndiswaConsultationApp.Models;
using AndiswaConsultationApp.Services;

namespace AndiswaConsultationApp.Pages;

public partial class ServicesPage : ContentPage
{
    public ServicesPage()
    {
        InitializeComponent();
    }

    // Runs when a service card is tapped.
    // "sender" is the card; its BindingContext is the ServiceItem that card shows.
    private async void OnServiceTapped(object? sender, TappedEventArgs e)
    {
        if (sender is BindableObject card && card.BindingContext is ServiceItem service)
            await AppNavigation.RequestServiceAsync(service.ContactService);
    }
}
