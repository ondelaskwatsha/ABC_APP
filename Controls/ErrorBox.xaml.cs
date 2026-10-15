// ============================================================================
// ErrorBox.xaml.cs  -  shows and hides the message box
// ----------------------------------------------------------------------------
// Pages use it like this:
//   ErrorBox.Show("Could not reach the server");       -> red box + Try again
//   ErrorBox.ShowInfo("You're offline ...");          -> soft navy-grey note
//   ErrorBox.Hide();                                   -> hide it
//   ErrorBox.Retry += ...                              -> what to do when Try again is tapped
// ============================================================================
namespace AndiswaConsultationApp.Controls;

public partial class ErrorBox : ContentView
{
    // An "event" lets the page decide what Try again does (usually: load the data again)
    public event EventHandler? Retry;

    public ErrorBox()
    {
        InitializeComponent();
    }

    // A real problem (red box with a "Try again" button)
    public void Show(string message)
    {
        Box.BackgroundColor = Color.FromArgb("#FFF4F4");
        Box.Stroke = Color.FromArgb("#E0B4B4");
        MessageLabel.TextColor = Color.FromArgb("#8A1F1F");
        MessageLabel.Text = message;
        RetryButton.IsVisible = true;
        IsVisible = true;
    }

    // Just a friendly note, for example "you are offline" (soft navy-grey box, no button)
    public void ShowInfo(string message)
    {
        Box.BackgroundColor = Color.FromArgb("#EEF1F4");
        Box.Stroke = Color.FromArgb("#DDE2E8");
        MessageLabel.TextColor = Color.FromArgb("#1B2A41");
        MessageLabel.Text = message;
        RetryButton.IsVisible = false;
        IsVisible = true;
    }

    public void Hide() => IsVisible = false;

    // Runs when Try again is tapped. "?.Invoke" = only call it if a page is listening.
    private void OnRetryClicked(object? sender, EventArgs e) => Retry?.Invoke(this, EventArgs.Empty);
}
