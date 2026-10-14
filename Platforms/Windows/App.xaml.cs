// ============================================================================
// Platforms/Windows/App.xaml.cs  -  Windows only. Connects Windows to MauiProgram.cs.
// ----------------------------------------------------------------------------
// You do not need to change this file. It is the same in every .NET MAUI app.
// ============================================================================
using Microsoft.UI.Xaml;

namespace AndiswaConsultationApp.WinUI;

public partial class App : MauiWinUIApplication
{
    public App()
    {
        this.InitializeComponent();
    }

    // Windows asks: "how do I build the app?" -> answer: use MauiProgram.CreateMauiApp()
    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}
