using AndiswaConsultationApp.Services;

namespace AndiswaConsultationApp.Pages;

public partial class ClientLoginPage : ContentPage
{
    private readonly ApiService _api;   // our link to the API

    public ClientLoginPage(ApiService api)
    {
        InitializeComponent();
        _api = api;
    }

    // Runs every time this page comes on screen
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Already logged in (from last time)? Go straight to the dashboard.
        await ClientSession.LoadAsync();
        if (ClientSession.IsLoggedIn)
        {
            // Dispatch = "do it right after this page has finished appearing"
            // (starting a new navigation in the middle of one can fail)
            Dispatcher.Dispatch(async () => await AppNavigation.GoToTabAsync(AppNavigation.ClientDashboardRoute));
            return;
        }

        // Start fresh: clear the password and any old error
        PasswordEntry.Text = "";
        HideError();
    }

    // "Next" on the keyboard after the email: jump to the password box
    private void OnEmailCompleted(object? sender, EventArgs e) => PasswordEntry.Focus();

    // "Show" / "Hide" next to the password
    private void OnShowPasswordTapped(object? sender, TappedEventArgs e)
    {
        PasswordEntry.IsPassword = !PasswordEntry.IsPassword;
        ShowPasswordLabel.Text = PasswordEntry.IsPassword ? "Show" : "Hide";
    }

    // The "Log in" button (and "Go" on the keyboard)
    private async void OnLoginClicked(object? sender, EventArgs e)
    {
        var email = (EmailEntry.Text ?? "").Trim();
        var password = PasswordEntry.Text ?? "";

        // Check the boxes are filled in before we bother the server
        if (email.Length == 0 || password.Length == 0)
        {
            ShowError("Please enter your email and password.");
            return;
        }

        HideError();
        SetBusy(true);
        try
        {
            var (result, error) = await _api.LoginAsync(email, password);

            if (result is null)
            {
                ShowError(error);
                return;
            }

            // Remember the login, then open the dashboard
            await ClientSession.SaveAsync(result.Token, result.Name, result.Email);
            PasswordEntry.Text = "";
            await AppNavigation.GoToTabAsync(AppNavigation.ClientDashboardRoute);
        }
        finally
        {
            SetBusy(false);
        }
    }

    // "Request a quote": no account yet, so send them to the Contact form
    private async void OnRequestQuoteClicked(object? sender, EventArgs e) =>
        await AppNavigation.GoToTabAsync("Contact");

    // ---- small helpers ----

    private void ShowError(string message)
    {
        ErrorLabel.Text = message;
        ErrorLabel.IsVisible = true;
    }

    private void HideError() => ErrorLabel.IsVisible = false;

    // While we wait for the server: spinner on, button off (so it can't be pressed twice)
    private void SetBusy(bool busy)
    {
        Busy.IsRunning = busy;
        Busy.IsVisible = busy;
        LoginButton.IsEnabled = !busy;
        LoginButton.Text = busy ? "Logging in..." : "Log in";
    }
}
