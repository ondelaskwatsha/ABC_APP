using System.Net.Mail;
using AndiswaConsultationApp.Services;

namespace AndiswaConsultationApp.Pages;

public partial class ClientRegisterPage : ContentPage
{
    private readonly ApiService _api;   // our link to the API

    public ClientRegisterPage(ApiService api)
    {
        InitializeComponent();
        _api = api;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Already logged in? There is nothing to register: go to the dashboard.
        await ClientSession.LoadAsync();
        if (ClientSession.IsLoggedIn)
        {
            Dispatcher.Dispatch(async () => await AppNavigation.GoToTabAsync(AppNavigation.ClientDashboardRoute));
            return;
        }

        // Never leave passwords on the screen
        PasswordEntry.Text = "";
        ConfirmEntry.Text = "";
        HideError();
    }

    // "Next" on the keyboard: jump to the next box
    private void OnNameCompleted(object? sender, EventArgs e) => EmailEntry.Focus();
    private void OnEmailCompleted(object? sender, EventArgs e) => PasswordEntry.Focus();
    private void OnPasswordCompleted(object? sender, EventArgs e) => ConfirmEntry.Focus();

    // "Show" / "Hide" next to the password (shows both password boxes)
    private void OnShowPasswordTapped(object? sender, TappedEventArgs e)
    {
        var hide = !PasswordEntry.IsPassword;
        PasswordEntry.IsPassword = hide;
        ConfirmEntry.IsPassword = hide;
        ShowPasswordLabel.Text = hide ? "Show" : "Hide";
    }

    // The "Create account" button (and "Go" on the keyboard)
    private async void OnRegisterClicked(object? sender, EventArgs e)
    {
        var name = (NameEntry.Text ?? "").Trim();
        var email = (EmailEntry.Text ?? "").Trim();
        var password = PasswordEntry.Text ?? "";
        var confirm = ConfirmEntry.Text ?? "";

        // Check the form here first, so the user gets an answer straight away.
        // (The API checks everything again, because a check in the app alone can be skipped.)
        var problem = CheckForm(name, email, password, confirm);
        if (problem is not null)
        {
            ShowError(problem);
            return;
        }

        HideError();
        SetBusy(true);
        try
        {
            var (result, error) = await _api.RegisterAsync(name, email, password);

            if (result is null)
            {
                ShowError(error);
                return;
            }

            // Account created and logged in: remember it and open the dashboard
            await ClientSession.SaveAsync(result.Token, result.Name, result.Email);
            NameEntry.Text = "";
            EmailEntry.Text = "";
            PasswordEntry.Text = "";
            ConfirmEntry.Text = "";
            await AppNavigation.GoToTabAsync(AppNavigation.ClientDashboardRoute);
        }
        finally
        {
            SetBusy(false);
        }
    }

    // "Log in": back to the login page
    private async void OnLoginClicked(object? sender, EventArgs e) =>
        await AppNavigation.GoToTabAsync(AppNavigation.ClientLoginRoute);

    // Returns what is wrong with the form, or null when everything is fine.
    // The same rules as the API (ClientController.CheckPasswordStrength).
    private static string? CheckForm(string name, string email, string password, string confirm)
    {
        if (name.Length < 2)
            return "Please enter your full name.";
        if (!IsValidEmail(email))
            return "Please enter a valid email address.";
        if (password.Length < 8)
            return "Your password must be at least 8 characters long.";
        if (!password.Any(char.IsLetter) || !password.Any(char.IsDigit))
            return "Your password must contain at least one letter and one number.";
        if (password != confirm)
            return "The two passwords do not match.";
        return null;
    }

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

    // ---- small helpers ----

    private void ShowError(string message)
    {
        ErrorLabel.Text = message;
        ErrorLabel.IsVisible = true;
    }

    private void HideError() => ErrorLabel.IsVisible = false;

    private void SetBusy(bool busy)
    {
        Busy.IsRunning = busy;
        Busy.IsVisible = busy;
        RegisterButton.IsEnabled = !busy;
        RegisterButton.Text = busy ? "Creating account..." : "Create account";
    }
}
