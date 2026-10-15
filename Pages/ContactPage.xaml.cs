// ============================================================================
// ContactPage.xaml.cs  -  the CODE behind the Contact page
// ----------------------------------------------------------------------------
// This file:
//   1. fills the "Service" drop-down with the same choices as the website
//   2. loads the contact details (email, phone, address, hours, owner)
//   3. checks the form when "Send Message" is tapped and sends it to the API
//      -> the API saves it in the website's quotes table
//      -> the admin sees it in admin.php under "Quote Requests"
//   4. when there is no internet, the message is saved on the phone and sent later
//   5. builds the "Quick answers" questions (the same answers as the website's chat assistant)
//   6. selects the service in the drop-down when you arrive from the Services page or a project
// ============================================================================
using System.Net.Mail;
using AndiswaConsultationApp.Models;
using AndiswaConsultationApp.Services;
using Microsoft.Maui.Controls.Shapes;

namespace AndiswaConsultationApp.Pages;

public partial class ContactPage : ContentPage
{
    // The same choices as the website's contact form, so the admin sees familiar values
    private static readonly string[] Services =
    {
        "Graphic Design",
        "Branding & Identity",
        "Social Media Design",
        "Web Design",
        "Business Consultation",
        "Other"
    };

    private readonly ApiService _api;   // our link to the API
    private StudioInfo? _studio;        // the contact details currently shown

    public ContactPage(ApiService api)
    {
        InitializeComponent();          // builds the layout from ContactPage.xaml
        _api = api;

        ServicePicker.ItemsSource = Services;                          // fill the drop-down
        ErrorBox.Retry += async (_, _) => await LoadStudioAsync();     // "Try again" button
        BuildFaq();                                                    // the "Quick answers" questions
    }

    // Runs every time the Contact tab comes on screen
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Did the user arrive by tapping "Request this service" on another page? Select that service.
        if (AppState.PendingService is { } wanted)
        {
            ServicePicker.SelectedItem = Services.Contains(wanted) ? wanted : "Other";
            AppState.PendingService = null;     // use it only once
        }

        // Try to send any messages that were saved while the phone was offline.
        // "_ =" means "start it but don't wait for it".
        _ = _api.SyncPendingAsync();

        // Load the details if we have none yet, or if last time we were offline
        if (_studio is null || _api.UsingOfflineData)
            await LoadStudioAsync();
    }

    // Loads the email, phone, address, hours and owner details
    private async Task LoadStudioAsync()
    {
        ErrorBox.Hide();
        try
        {
            // Show the saved copy instantly, then the latest from the server
            if (_studio is null)
                Show(await _api.GetSavedStudioAsync());

            Show(await _api.GetStudioAsync());

            if (_api.UsingOfflineData)
                ErrorBox.ShowInfo("You're offline. You can still write to us: your message will be sent when you're back online.");
        }
        catch (Exception ex)
        {
            ErrorBox.Show(ApiService.FriendlyError(ex));
        }
    }

    // Puts the details on screen. After BindingContext is set, {Binding Email} etc. show the values.
    private void Show(StudioInfo studio)
    {
        _studio = studio;
        BindingContext = studio;
        Footer.ApplyStudio(studio);     // the footer shows the social media buttons
    }

    // Creates the "Quick answers": tap a question to open or close its answer
    private void BuildFaq()
    {
        FaqLayout.Children.Clear();

        foreach (var item in SiteContent.Faq)
        {
            var sign = new Label
            {
                Text = "+",
                FontSize = 22,
                TextColor = Color.FromArgb("#3E6690"),
                VerticalOptions = LayoutOptions.Center
            };

            var question = new Label
            {
                Text = item.Question,
                FontSize = 15,
                FontAttributes = FontAttributes.Bold,
                TextColor = Color.FromArgb("#1B2A41"),
                VerticalOptions = LayoutOptions.Center
            };

            // The answer is hidden until the question is tapped
            var answer = new Label
            {
                Text = item.Answer,
                FontSize = 14,
                LineHeight = 1.45,
                TextColor = Color.FromArgb("#5B6472"),
                IsVisible = false,
                Margin = new Thickness(0, 10, 0, 0)
            };

            // The question on the left, the + / - sign on the right
            var row = new Grid
            {
                ColumnDefinitions =
                {
                    new ColumnDefinition(GridLength.Star),
                    new ColumnDefinition(GridLength.Auto)
                },
                ColumnSpacing = 10
            };
            row.Add(question, 0, 0);
            row.Add(sign, 1, 0);

            var card = new Border
            {
                BackgroundColor = Colors.White,
                Stroke = Color.FromArgb("#DDE2E8"),
                StrokeThickness = 1,
                StrokeShape = new RoundRectangle { CornerRadius = 14 },
                Padding = new Thickness(16, 14),
                Content = new VerticalStackLayout { Children = { row, answer } }
            };

            var tap = new TapGestureRecognizer();
            tap.Tapped += (_, _) =>
            {
                answer.IsVisible = !answer.IsVisible;
                sign.Text = answer.IsVisible ? "–" : "+";
            };
            card.GestureRecognizers.Add(tap);

            FaqLayout.Children.Add(card);
        }
    }

    // Runs when "Send Message" is tapped
    private async void OnSendClicked(object? sender, EventArgs e)
    {
        // ---- Step 1: read what was typed. "?.Trim() ?? """ = remove spaces, or empty if nothing typed
        var first = FirstNameEntry.Text?.Trim() ?? "";
        var last = LastNameEntry.Text?.Trim() ?? "";
        var email = EmailEntry.Text?.Trim() ?? "";
        var phone = PhoneEntry.Text?.Trim() ?? "";
        var service = ServicePicker.SelectedItem as string ?? "";
        var message = MessageEditor.Text?.Trim() ?? "";

        // ---- Step 2: check the form BEFORE sending. "return" stops here if something is wrong.
        //      (The API checks again, because you can never trust data from outside.)
        if (first.Length == 0)
        {
            await DisplayAlert("Missing name", "Please enter your first name.", "OK");
            return;
        }

        if (!IsValidEmail(email))
        {
            await DisplayAlert("Check your email", "Please enter a valid email address.", "OK");
            return;
        }

        if (service.Length == 0)
        {
            await DisplayAlert("Choose a service", "Please select the service you need.", "OK");
            return;
        }

        if (message.Length == 0)
        {
            await DisplayAlert("Missing message", "Please tell us a little about your project.", "OK");
            return;
        }

        // ---- Step 3: send. Disable the button so it can't be tapped twice, and show the spinner.
        SendButton.IsEnabled = false;
        Busy.IsVisible = Busy.IsRunning = true;

        var result = await _api.SendQuoteAsync(new QuoteRequest
        {
            Name = $"{first} {last}".Trim(),    // the website stores one "name", so we join first + last
            Email = email,
            Phone = phone,
            Service = service,
            Message = message
        });

        Busy.IsVisible = Busy.IsRunning = false;
        SendButton.IsEnabled = true;

        // ---- Step 4: tell the user what happened
        if (result.Success)
        {
            // Clear the form
            FirstNameEntry.Text = LastNameEntry.Text = EmailEntry.Text = PhoneEntry.Text = string.Empty;
            MessageEditor.Text = string.Empty;
            ServicePicker.SelectedIndex = -1;   // -1 = nothing selected

            if (result.Queued)
                await DisplayAlert("Saved on your phone",
                    "There is no connection right now. Your message is saved and will be sent automatically when you are back online.",
                    "OK");
            else
                await DisplayAlert("Message sent",
                    "Your quote request was sent successfully. Thank you! Our team will review it and contact you with the next steps.",
                    "OK");
        }
        else
        {
            await DisplayAlert("Could not send", result.Error, "OK");
        }
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

    // Tapping the email opens the phone's email app with the address filled in
    private async void OnEmailTapped(object? sender, TappedEventArgs e)
    {
        if (_studio is null) return;
        try { await Launcher.OpenAsync($"mailto:{_studio.Email}"); } catch { }
    }

    // Tapping the phone number opens the dialler (spaces removed first)
    private async void OnPhoneTapped(object? sender, TappedEventArgs e)
    {
        if (_studio is null) return;
        try { await Launcher.OpenAsync($"tel:{_studio.Phone.Replace(" ", "")}"); } catch { }
    }
}
