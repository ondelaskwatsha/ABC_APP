// Pages use it like this:
//   ErrorBox.Show("Could not reach the server");   -> red box + Try again
//   ErrorBox.ShowInfo("You're offline ...");       -> soft note
//   ErrorBox.Hide();                                -> hide it
//   ErrorBox.Retry += ...                           -> what to do when Try again is tapped
// The colours are theme colours (see ThemeService.cs), so they follow dark / light mode.
namespace AndiswaConsultationApp.Controls;

public partial class ErrorBox : ContentView
{
    public event EventHandler? Retry;

    public ErrorBox()
    {
        InitializeComponent();
    }

    // A real problem (red box with a "Try again" button)
    public void Show(string message)
    {
        Box.SetDynamicResource(VisualElement.BackgroundColorProperty, "ErrorBg");
        Box.SetDynamicResource(Border.StrokeProperty, "ErrorLine");
        MessageLabel.SetDynamicResource(Label.TextColorProperty, "ErrorText");
        MessageLabel.Text = message;
        RetryButton.IsVisible = true;
        IsVisible = true;
    }

    // Just a friendly note, for example "you are offline" (soft box, no button)
    public void ShowInfo(string message)
    {
        Box.SetDynamicResource(VisualElement.BackgroundColorProperty, "Mist");
        Box.SetDynamicResource(Border.StrokeProperty, "Line");
        MessageLabel.SetDynamicResource(Label.TextColorProperty, "Ink");
        MessageLabel.Text = message;
        RetryButton.IsVisible = false;
        IsVisible = true;
    }

    public void Hide() => IsVisible = false;

    private void OnRetryClicked(object? sender, EventArgs e) => Retry?.Invoke(this, EventArgs.Empty);
}