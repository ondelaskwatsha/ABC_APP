// ============================================================================
// MainApplication.cs  -  Android only. Connects Android to MauiProgram.cs.
// ----------------------------------------------------------------------------
// You do not need to change this file. It is the same in every .NET MAUI app.
// ============================================================================
using Android.App;
using Android.Runtime;

namespace AndiswaConsultationApp;

[Application]
public class MainApplication : MauiApplication
{
    public MainApplication(IntPtr handle, JniHandleOwnership ownership)
        : base(handle, ownership)
    {
    }

    // Android asks: "how do I build the app?" -> answer: use MauiProgram.CreateMauiApp()
    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}
