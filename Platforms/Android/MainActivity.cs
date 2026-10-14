// ============================================================================
// MainActivity.cs  -  Android only. The screen Android opens when you tap the app icon.
// ----------------------------------------------------------------------------
// You do not need to change this file. It is the same in every .NET MAUI app.
// MainLauncher = true  -> this is the screen that starts the app
// Theme = SplashTheme  -> shows the light blue start-up screen while the app loads
// ============================================================================
using Android.App;
using Android.Content.PM;

namespace AndiswaConsultationApp;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop,
    ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode |
                           ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
}
