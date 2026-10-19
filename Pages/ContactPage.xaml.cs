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
        InitializeComponent();
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

        // Try to send any messages that were saved while the phone was offline
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

    private void Show(StudioInfo studio)
    {
        _studio = studio;
        BindingContext = studio;
        Footer.ApplyStudio(studio);
    }

    // Creates the "Quick answers": tap a question to open or close its answer.
    // ".Themed(...)" ties a colour to the theme so it follows dark / light mode.
    private void BuildFaq()
    {
        FaqLayout.Children.Clear();

        foreach (var item in SiteContent.Faq)
        {
            var sign = new Label
            {
                Text = "+",
                FontSize = 22,
                VerticalOptions = LayoutOptions.Center
            }.Themed(Label.TextColorProperty, "Steel");

            var question = new Label
            {
                Text = item.Question,
                FontSize = 15,
                FontAttributes = FontAttributes.Bold,
                VerticalOptions = LayoutOptions.Center
            }.Themed(Label.TextColorProperty, "Navy");

            // The answer is hidden until the question is tapped
            var answer = new Label
            {
                Text = item.Answer,
                FontSize = 14,
                LineHeight = 1.45,
                IsVisible = false,
                Margin = new Thickness(0, 10, 0, 0)
            }.Themed(Label.TextColorProperty, "Muted");

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
                StrokeThickness = 1,
                StrokeShape = new RoundRectangle { CornerRadius = 14 },
                Padding = new Thickness(16, 14),
                Content = new VerticalStackLayout { Children = { row, answer } }
            }
            .Themed(Border.StrokeProperty, "Line")
            .Themed(VisualElement.BackgroundColorProperty, "Surface");

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
        // Step 1: read what was typed
        var first = FirstNameEntry.Text?.Trim() ?? "";
        var last = LastNameEntry.Text?.Trim() ?? "";
        var email = EmailEntry.Text?.Trim() ?? "";
        var phone = PhoneEntry.Text?.Trim() ?? "";
        var service = ServicePicker.SelectedItem as string ?? "";
        var message = MessageEditor.Text?.Trim() ?? "";

        // Step 2: check the form BEFORE sending
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

        // Step 3: send. Disable the button so it can't be tapped twice, and show the spinner.
        SendButton.IsEnabled = false;
        Busy.IsVisible = Busy.IsRunning = true;

        var result = await _api.SendQuoteAsync(new QuoteRequest
        {
            Name = $"{first} {last}".Trim(),
            Email = email,
            Phone = phone,
            Service = service,
            Message = message
        });

        Busy.IsVisible = Busy.IsRunning = false;
        SendButton.IsEnabled = true;

        // Step 4: tell the user what happened
        if (result.Success)
        {
            FirstNameEntry.Text = LastNameEntry.Text = EmailEntry.Text = PhoneEntry.Text = string.Empty;
            MessageEditor.Text = string.Empty;
            ServicePicker.SelectedIndex = -1;

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

    private async void OnEmailTapped(object? sender, TappedEventArgs e)
    {
        if (_studio is null) return;
        try { await Launcher.OpenAsync($"mailto:{_studio.Email}"); } catch { }
    }

    private async void OnPhoneTapped(object? sender, TappedEventArgs e)
    {
        if (_studio is null) return;
        try { await Launcher.OpenAsync($"tel:{_studio.Phone.Replace(" ", "")}"); } catch { }
    }
}