// ============================================================================
// ClientSession.cs  -  remembers WHO is logged in to the client portal
// ----------------------------------------------------------------------------
// After a good login the API sends back a "token" (a long secret text that
// proves who you are). We keep it, with the client's name and email, so:
//   - the dashboard can ask the API for "my quotes" and "my projects"
//   - the client stays logged in when the app is closed and opened again
//
// The token is saved in SecureStorage (the phone's protected storage).
// If SecureStorage is not available (it can fail on Windows when the app is
// not packaged), we fall back to Preferences so the app keeps working.
// ============================================================================
namespace AndiswaConsultationApp.Services;

public static class ClientSession
{
    private const string TokenKey = "client_token";
    private const string NameKey = "client_name";
    private const string EmailKey = "client_email";

    private static bool _loaded;   // true once we have read the saved values

    public static string? Token { get; private set; }
    public static string Name { get; private set; } = "";
    public static string Email { get; private set; } = "";

    public static bool IsLoggedIn => !string.IsNullOrEmpty(Token);

    /// <summary>Reads the saved login (only the first time it is called).</summary>
    public static async Task LoadAsync()
    {
        if (_loaded) return;
        _loaded = true;

        Token = await ReadSecretAsync(TokenKey);
        Name = Preferences.Default.Get(NameKey, "");
        Email = Preferences.Default.Get(EmailKey, "");
    }

    /// <summary>Called after a successful login.</summary>
    public static async Task SaveAsync(string token, string name, string email)
    {
        Token = token;
        Name = name;
        Email = email;
        _loaded = true;

        await WriteSecretAsync(TokenKey, token);
        Preferences.Default.Set(NameKey, name);
        Preferences.Default.Set(EmailKey, email);
    }

    /// <summary>Log out: forget everything.</summary>
    public static void Clear()
    {
        Token = null;
        Name = "";
        Email = "";
        _loaded = true;

        try { SecureStorage.Default.Remove(TokenKey); } catch { /* not available: nothing saved there */ }
        Preferences.Default.Remove(TokenKey);
        Preferences.Default.Remove(NameKey);
        Preferences.Default.Remove(EmailKey);
    }

    // ---- helpers: SecureStorage first, Preferences if SecureStorage fails ----

    private static async Task<string?> ReadSecretAsync(string key)
    {
        try
        {
            var value = await SecureStorage.Default.GetAsync(key);
            if (!string.IsNullOrEmpty(value))
                return value;
        }
        catch { /* SecureStorage not available on this device */ }

        var fallback = Preferences.Default.Get(key, "");
        return string.IsNullOrEmpty(fallback) ? null : fallback;
    }

    private static async Task WriteSecretAsync(string key, string value)
    {
        try
        {
            await SecureStorage.Default.SetAsync(key, value);
        }
        catch
        {
            Preferences.Default.Set(key, value);
        }
    }
}
