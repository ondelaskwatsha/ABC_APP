// ============================================================================
// NewsletterBox.xaml.cs  -  checks the email and thanks the visitor
// ----------------------------------------------------------------------------
// This does exactly what the website does (about.js / services.js): it checks that an email
// was typed and looks valid, then says "Thank you for subscribing!".
// NOTE FOR THE TEAM: like the website, it does not save the email anywhere yet.
// To really collect emails, add an API endpoint (for example POST /api/subscribers) and call it here.
// ============================================================================
using System.Net.Mail;

namespace AndiswaConsultationApp.Controls;

public partial class NewsletterBox : ContentView
{
    public NewsletterBox()
    {
        InitializeComponent();
    }

    private async void OnSubscribeClicked(object? sender, EventArgs e)
    {
        var email = EmailEntry.Text?.Trim() ?? "";

        if (Shell.Current is null)
            return;

        if (email.Length == 0)
        {
            await Shell.Current.DisplayAlert("Newsletter", "Please enter your email address.", "OK");
            return;
        }

        if (!IsValidEmail(email))
        {
            await Shell.Current.DisplayAlert("Newsletter", "Please enter a valid email address.", "OK");
            return;
        }

        EmailEntry.Text = string.Empty;
        await Shell.Current.DisplayAlert("Newsletter", "Thank you for subscribing!", "OK");
    }

    // Checks that an email looks like name@something.com
    private static bool IsValidEmail(string email)
    {
        try
        {
            return new MailAddress(email).Address == email;
        }
        catch
        {
            return false;
        }
    }
}
