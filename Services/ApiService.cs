
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using AndiswaConsultationApp.Models;

namespace AndiswaConsultationApp.Services;

public class ApiService
{
    // JSON settings
    private static readonly JsonSerializerOptions JsonOpts = new(JsonSerializerDefaults.Web);

    private readonly HttpClient _http;                     // the object that sends web requests
    private readonly SemaphoreSlim _syncLock = new(1, 1);  // a "one at a time" lock for the pending messages file
    private StudioInfo? _studio;                           // kept in memory so Home and Contact share one download

    private static string StudioCachePath => Path.Combine(FileSystem.AppDataDirectory, "studio.json");
    private static string PendingPath => Path.Combine(FileSystem.AppDataDirectory, "pending-quotes.json");

    /// <summary>True when the last load used saved data instead of the server (we are offline).</summary>
    public bool UsingOfflineData { get; private set; }

    public ApiService()
    {
        _http = new HttpClient
        {
            BaseAddress = new Uri(ApiConfig.BaseUrl),   // every request starts with this address
            // Wait up to 15 seconds: the free Azure plan can take a moment to wake up
            Timeout = TimeSpan.FromSeconds(15)
        };

        // Listen for the phone getting its internet back. When it does,
        // send any Contact messages that were waiting.
        Connectivity.Current.ConnectivityChanged += async (_, e) =>
        {
            if (e.NetworkAccess == NetworkAccess.Internet)
                await SyncPendingAsync();
        };
    }

    // ------------------------------------------------------------------
    // STUDIO INFORMATION (Home + Contact pages)
    // ------------------------------------------------------------------

    /// <summary>
    /// Instant and never uses the internet: returns the saved copy,
    /// or the starter copy (seed.json) if nothing was saved yet.
    /// Pages call this first so the screen is never empty.
    /// </summary>
    public async Task<StudioInfo> GetSavedStudioAsync()
    {
        // 1) Is there a saved copy from last time?
        if (File.Exists(StudioCachePath))
        {
            try
            {
                var cached = JsonSerializer.Deserialize<StudioInfo>(await File.ReadAllTextAsync(StudioCachePath), JsonOpts);
                if (cached is not null)
                    return cached;
            }
            catch (JsonException) { /* the file is damaged: use the starter copy instead */ }
        }

        // 2) No saved copy: open seed.json, which is packed inside the app
        await using var stream = await FileSystem.OpenAppPackageFileAsync("seed.json");
        return await JsonSerializer.DeserializeAsync<StudioInfo>(stream, JsonOpts)
               ?? throw new InvalidOperationException("The built-in starter information is missing.");
    }

    /// <summary>
    /// Gets the latest information from the API.
    /// If there is no internet, it quietly returns the saved copy instead.
    /// forceRefresh = true means "download again even if we already have it" (pull to refresh).
    /// </summary>
    public async Task<StudioInfo> GetStudioAsync(bool forceRefresh = false)
    {
        // Already downloaded during this session? Reuse it (saves data and time)
        if (_studio is not null && !forceRefresh && !UsingOfflineData)
            return _studio;

        try
        {
            // Ask the API: GET {BaseUrl}studio  and turn the JSON answer into a StudioInfo
            var fresh = await _http.GetFromJsonAsync<StudioInfo>("studio", JsonOpts)
                        ?? throw new JsonException("The server sent an empty answer.");

            // Save a copy on the phone for next time we are offline
            await File.WriteAllTextAsync(StudioCachePath, JsonSerializer.Serialize(fresh, JsonOpts));
            _studio = fresh;
            AppState.Studio = fresh;          // the footer on every page reads this
            UsingOfflineData = false;

            // We are online, so try to send any waiting messages in the background.
            // The "_ =" means "start it, but don't wait for it to finish".
            _ = SyncPendingAsync();
            return fresh;
        }
        // These three errors mean "no internet", "too slow" or "strange answer"
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or JsonException)
        {
            UsingOfflineData = true;
            _studio = await GetSavedStudioAsync();
            AppState.Studio = _studio;
            return _studio;
        }
    }

    // ------------------------------------------------------------------
    // SENDING THE CONTACT FORM (goes into the website's "quotes" table)
    // ------------------------------------------------------------------

    public async Task<SendResult> SendQuoteAsync(QuoteRequest quote)
    {
        try
        {
            // POST {BaseUrl}quotes  with the form as JSON
            var response = await _http.PostAsJsonAsync("quotes", quote, JsonOpts);

            // 200-299 = success
            if (response.IsSuccessStatusCode)
                return new SendResult(true, false, "");

            // 429 = the spam limit in the API (more than 5 messages in a minute)
            if (response.StatusCode == HttpStatusCode.TooManyRequests)
                return new SendResult(false, false, "You have sent several messages in a short time. Please wait a minute and try again.");

            // The server answered but said no (for example a bad email).
            // Show its reason and do NOT save it for later, because it would fail again.
            return new SendResult(false, false, await ReadServerErrorAsync(response));
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
        {
            // No connection: keep the message on the phone and send it when we are back online
            await SavePendingAsync(quote);
            return new SendResult(true, true, "");
        }
    }

    // ------------------------------------------------------------------
    // CLIENT PORTAL (Account tab)
    // ------------------------------------------------------------------

    /// <summary>
    /// Logs a client in with email + password (POST {BaseUrl}client/login).
    /// Returns the login details, or an error sentence to show on the page.
    /// </summary>
    public Task<(ClientLoginResult? Result, string Error)> LoginAsync(string email, string password) =>
        PostForLoginAsync("client/login", new ClientLoginRequest { Email = email, Password = password });

    /// <summary>
    /// Creates a new client account (POST {BaseUrl}client/register) and logs it in.
    /// The password travels to the API, which saves only a bcrypt HASH of it.
    /// </summary>
    public Task<(ClientLoginResult? Result, string Error)> RegisterAsync(string name, string email, string password) =>
        PostForLoginAsync("client/register", new ClientRegisterRequest { Name = name, Email = email, Password = password });

    // Login and register both send JSON and get back a token, so they share this code
    private async Task<(ClientLoginResult? Result, string Error)> PostForLoginAsync<T>(string address, T body)
    {
        try
        {
            var response = await _http.PostAsJsonAsync(address, body, JsonOpts);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ClientLoginResult>(JsonOpts);
                return result is null || result.Token.Length == 0
                    ? (null, "The server sent an empty answer. Please try again.")
                    : (result, "");
            }

            // 429 = too many tries in one minute (the API's password-guessing protection)
            if (response.StatusCode == HttpStatusCode.TooManyRequests)
                return (null, "Too many attempts. Please wait a minute and try again.");

            // 400 / 401 / 409: show the API's own sentence, e.g. "Incorrect email or password."
            return (null, await ReadServerErrorAsync(response));
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or JsonException)
        {
            return (null, FriendlyError(ex));
        }
    }

    /// <summary>
    /// SINGLE SIGN-ON with Google.
    ///  1. Makes a random secret "session" code.
    ///  2. Opens the browser at {BaseUrl}sso/google?session=CODE, where the client signs in with Google.
    ///  3. Asks {BaseUrl}sso/result?session=CODE every 2 seconds until the API has the login ready.
    /// Gives up after 5 minutes, or when the page cancels (the "Cancel" button).
    /// </summary>
    public async Task<(ClientLoginResult? Result, string Error)> GoogleSignInAsync(CancellationToken cancel)
    {
        // A random code nobody can guess (32 random bytes = 43 letters/numbers)
        var session = Convert.ToBase64String(System.Security.Cryptography.RandomNumberGenerator.GetBytes(32))
                             .TrimEnd('=').Replace('+', '-').Replace('/', '_');

        try
        {
            // Open the browser (on Android a browser tab on top of the app, on Windows the normal browser)
            var signInPage = new Uri(new Uri(ApiConfig.BaseUrl), $"sso/google?session={session}");
            await Browser.Default.OpenAsync(signInPage, BrowserLaunchMode.SystemPreferred);

            var giveUpAt = DateTime.UtcNow.AddMinutes(5);
            while (DateTime.UtcNow < giveUpAt)
            {
                await Task.Delay(TimeSpan.FromSeconds(2), cancel);   // wait, then ask again

                try
                {
                    var response = await _http.GetAsync($"sso/result?session={session}", cancel);

                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        var result = await response.Content.ReadFromJsonAsync<ClientLoginResult>(JsonOpts, cancel);
                        if (result is not null && result.Token.Length > 0)
                            return (result, "");
                    }
                    // 202 = "not finished yet": just loop and ask again
                }
                catch (Exception ex) when (ex is HttpRequestException ||
                                           (ex is TaskCanceledException && !cancel.IsCancellationRequested))
                {
                    // A short internet hiccup: keep trying until the 5 minutes are over
                }
            }

            return (null, "Google sign-in took too long. Please try again.");
        }
        catch (OperationCanceledException) when (cancel.IsCancellationRequested)
        {
            return (null, "");   // the user pressed Cancel: no error to show
        }
        catch (Exception ex)
        {
            return (null, FriendlyError(ex));
        }
    }

    /// <summary>
    /// Gets the logged-in client's quote requests and projects (GET {BaseUrl}client/dashboard).
    /// Throws UnauthorizedAccessException when the login has expired, so the page can log out.
    /// </summary>
    public async Task<ClientDashboard> GetDashboardAsync(string token)
    {
        // The token goes in the "Authorization" header of THIS request only
        using var request = new HttpRequestMessage(HttpMethod.Get, "client/dashboard");
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        using var response = await _http.SendAsync(request);

        if (response.StatusCode == HttpStatusCode.Unauthorized)
            throw new UnauthorizedAccessException(await ReadServerErrorAsync(response));

        response.EnsureSuccessStatusCode();   // any other error becomes an HttpRequestException

        return await response.Content.ReadFromJsonAsync<ClientDashboard>(JsonOpts)
               ?? throw new JsonException("The server sent an empty answer.");
    }

    // The API sends errors like {"error": "Please enter a valid email address."}.
    // This reads the sentence inside "error" so the app can show it.
    private static async Task<string> ReadServerErrorAsync(HttpResponseMessage response)
    {
        try
        {
            var body = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(body);
            if (doc.RootElement.TryGetProperty("error", out var err))
                return err.GetString() ?? "Something went wrong.";
        }
        catch (JsonException) { }

        // No reason given: at least show the error number, it helps to find the problem
        return $"Something went wrong (error {(int)response.StatusCode}). Please try again.";
    }

    // ------------------------------------------------------------------
    // MESSAGES WAITING TO BE SENT (saved in pending-quotes.json on the phone)
    // ------------------------------------------------------------------

    // Reads the list of waiting messages from the phone (empty list if there are none)
    private static List<QuoteRequest> LoadPending()
    {
        if (!File.Exists(PendingPath))
            return new();

        try
        {
            return JsonSerializer.Deserialize<List<QuoteRequest>>(File.ReadAllText(PendingPath), JsonOpts) ?? new();
        }
        catch (JsonException)
        {
            return new();
        }
    }

    // Adds one message to the waiting list
    private async Task SavePendingAsync(QuoteRequest quote)
    {
        await _syncLock.WaitAsync();       // make sure no one else is using the file right now
        try
        {
            var list = LoadPending();
            list.Add(quote);
            await File.WriteAllTextAsync(PendingPath, JsonSerializer.Serialize(list, JsonOpts));
        }
        finally
        {
            _syncLock.Release();           // let others use the file again
        }
    }

    /// <summary>Tries to send every waiting message. Stops at the first failure and tries again later.</summary>
    public async Task SyncPendingAsync()
    {
        // WaitAsync(0) = "only continue if nobody else is syncing right now"
        if (!await _syncLock.WaitAsync(0))
            return;

        try
        {
            var list = LoadPending();
            if (list.Count == 0)
                return;                                  // nothing to send

            var stillWaiting = new List<QuoteRequest>(list);

            foreach (var quote in list)
            {
                try
                {
                    var response = await _http.PostAsJsonAsync("quotes", quote, JsonOpts);
                    var code = (int)response.StatusCode;

                    if (response.IsSuccessStatusCode)
                    {
                        stillWaiting.Remove(quote);          // sent successfully
                    }
                    else if (code is >= 400 and < 500 && response.StatusCode != HttpStatusCode.TooManyRequests
                                                      && response.StatusCode != HttpStatusCode.RequestTimeout)
                    {
                        stillWaiting.Remove(quote);          // the server refused it for good (bad data): drop it
                    }
                    else
                    {
                        break;                               // server busy or down: try again later
                    }
                }
                catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
                {
                    break;   // still offline: try again later
                }
            }

            // Save what is left. If everything was sent, delete the file.
            if (stillWaiting.Count == 0)
            {
                if (File.Exists(PendingPath))
                    File.Delete(PendingPath);
            }
            else
            {
                await File.WriteAllTextAsync(PendingPath, JsonSerializer.Serialize(stillWaiting, JsonOpts));
            }
        }
        catch
        {
            // Syncing is "best effort". If anything goes wrong it will be tried again next time.
        }
        finally
        {
            _syncLock.Release();
        }
    }

    // Turns technical errors into a sentence a normal person can understand
    public static string FriendlyError(Exception ex) => ex switch
    {
        HttpRequestException or TaskCanceledException =>
            "Could not reach the server. Check your internet connection and try again.",
        JsonException => "The server sent back data the app could not read.",
        _ => ex.Message
    };
}

/// <summary>The result of pressing "Send Message".</summary>
/// <param name="Success">The message was accepted (sent now, or saved to send later).</param>
/// <param name="Queued">True when there was no connection and the message was saved on the phone.</param>
/// <param name="Error">What went wrong, if Success is false.</param>
public record SendResult(bool Success, bool Queued, string Error);
