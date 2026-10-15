
namespace AndiswaConsultationApp.Models;

// One number on the Home page, for example Value "50+" and Label "Projects Completed"
public class StatItem
{
    public string Value { get; set; } = "";
    public string Label { get; set; } = "";
}

public class StudioInfo
{
    public string Name { get; set; } = "";
    public string Tagline { get; set; } = "";
    public string HeroText { get; set; } = "";
    public string AboutText1 { get; set; } = "";
    public string AboutText2 { get; set; } = "";
    public string Email { get; set; } = "";
    public string Phone { get; set; } = "";
    public string Address { get; set; } = "";
    public string Hours { get; set; } = "";
    public string OwnerName { get; set; } = "";
    public string OwnerTitle { get; set; } = "";
    public string InstagramUrl { get; set; } = "";
    public string LinkedInUrl { get; set; } = "";
    public string TwitterUrl { get; set; } = "";
    public List<StatItem> Stats { get; set; } = new();

    // The first letters of the owner's name, shown in the round circle on the Contact page.
    // "Andiswa Phewa" -> "AP". It is calculated, not downloaded.
    public string OwnerInitials =>
        string.Concat(OwnerName.Split(' ', StringSplitOptions.RemoveEmptyEntries)   // split into words
                               .Take(2)                                                // first two words
                               .Select(p => char.ToUpper(p[0])));                      // first letter of each
}

/// <summary>
/// What the Contact form sends to the API (POST /api/quotes).
/// The API saves it in the website's "quotes" table, so the admin sees it.
/// </summary>
public class QuoteRequest
{
    public string Name { get; set; } = "";
    public string Email { get; set; } = "";
    public string Phone { get; set; } = "";
    public string Service { get; set; } = "";
    public string Message { get; set; } = "";
}
