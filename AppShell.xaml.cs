// ============================================================================
// AppShell.xaml.cs  -  the code behind the tab bar
// ----------------------------------------------------------------------------
// Everything about the tabs is in AppShell.xaml. This file only loads it.
// ============================================================================
namespace AndiswaConsultationApp;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();   // builds the tab bar described in AppShell.xaml
    }
}
