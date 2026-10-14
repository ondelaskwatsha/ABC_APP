
namespace AndiswaConsultationApp.Services;

public static class ApiConfig
{
    // The address of your API on Azure. Example:
    // "https://andiswa-api-1234.azurewebsites.net/api/"
    public const string AzureApiUrl = "https://YOUR-API-NAME.azurewebsites.net/api/";

    // true  = talk to the API on Azure (needs AzureApiUrl filled in)
    // false = talk to the API running on your own PC
    public static bool UseAzure { get; set; } = false;

    // Only for testing on a REAL Android phone with the API on your own PC:
    // open Command Prompt on the PC, type "ipconfig" and copy the IPv4 Address here.
    public const string PhoneLanIp = "192.168.1.100";

    // The port the API uses on your PC (see the API's Properties/launchSettings.json)
    private const int LocalPort = 5080;

    // Works out the right address every time the app asks for it
    public static string BaseUrl
    {
        get
        {
            // If UseAzure is on AND you replaced the placeholder text, use Azure
            if (UseAzure && !AzureApiUrl.Contains("YOUR-API-NAME"))
                return AzureApiUrl;

            // Otherwise use the API on your own PC.
            // "#if ANDROID" means: this part is only used when the app runs on Android.
#if ANDROID
            // The Android EMULATOR is like a separate computer. It reaches your PC
            // through the special address 10.0.2.2. A real phone needs your PC's Wi-Fi address.
            return DeviceInfo.DeviceType == DeviceType.Virtual
                ? $"http://10.0.2.2:{LocalPort}/api/"
                : $"http://{PhoneLanIp}:{LocalPort}/api/";
#else
            // On Windows the app and the API are on the same PC, so "localhost" works
            return $"http://localhost:{LocalPort}/api/";
#endif
        }
    }
}
