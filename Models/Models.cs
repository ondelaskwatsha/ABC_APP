namespace AndiswaConsultationApp.Models;

// One number on the Home page, for example Value "50+" and Label "Projects Completed"
public class StatItem
{
    public string Value { get; set; } = "";
    public string Label { get; set; } = "";
}

/// <summary>
/// Everything the Home and Contact pages show that can change.
/// It comes from the API (GET /api/studio), which reads the website's "settings" table.
/// = "" means "start as empty text" so the page never crashes on a missing value.
/// </summary>
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
    public string FacebookUrl { get; set; } = "";
    public string InstagramUrl { get; set; } = "";
    public string TwitterUrl { get; set; } = "";
    public string LinkedInUrl { get; set; } = "";
    public string YouTubeUrl { get; set; } = "";
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

// ============================================================================
// CLIENT PORTAL  (Account tab: login + dashboard, same accounts as the website)
// ============================================================================

/// <summary>What the login page sends to the API (POST /api/client/login).</summary>
public class ClientLoginRequest
{
    public string Email { get; set; } = "";
    public string Password { get; set; } = "";
}

/// <summary>What the register page sends to the API (POST /api/client/register).</summary>
public class ClientRegisterRequest
{
    public string Name { get; set; } = "";
    public string Email { get; set; } = "";
    public string Password { get; set; } = "";
}

/// <summary>What the API sends back after a good login (normal, register or Google).</summary>
public class ClientLoginResult
{
    public string Token { get; set; } = "";
    public string Name { get; set; } = "";
    public string Email { get; set; } = "";
}

/// <summary>One quote request on the dashboard ("Your quote requests").</summary>
public class ClientQuote
{
    public uint Id { get; set; }
    public string Service { get; set; } = "";
    public string Message { get; set; } = "";
    public string Status { get; set; } = "New";
    public DateTime? CreatedAt { get; set; }
}

/// <summary>One project on the dashboard ("Your projects").</summary>
public class ClientProject
{
    public uint Id { get; set; }
    public string Title { get; set; } = "";
    public string Status { get; set; } = "";
    public bool HasStatus => Status.Length > 0;
}

/// <summary>Everything the dashboard shows (GET /api/client/dashboard).</summary>
public class ClientDashboard
{
    public string Name { get; set; } = "";
    public string Email { get; set; } = "";
    public List<ClientQuote> Quotes { get; set; } = new();
    public List<ClientProject> Projects { get; set; } = new();
}

// ============================================================================
// WEBSITE CONTENT  (the pages below read this; it is copied from the website)
// ============================================================================

/// <summary>One service on the Services page (services.html).</summary>
public class ServiceItem
{
    public string Icon { get; set; } = "";          // emoji, same as the website
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    // Which choice of the Contact form's drop-down this service belongs to
    public string ContactService { get; set; } = "Other";
}

/// <summary>One "Why Clients Choose Us" reason (services.html).</summary>
public class ReasonItem
{
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
}

/// <summary>One project on the Portfolio page (projects.html).</summary>
public class ProjectItem
{
    public string Title { get; set; } = "";
    public string Category { get; set; } = "";      // label shown on the card, e.g. "Branding"
    public string Filter { get; set; } = "";        // filter button it belongs to: branding / flyers / posters / social
    public string Blurb { get; set; } = "";         // the short line under the title
    public string Description { get; set; } = "";   // the longer text on the detail page
    public string Image { get; set; } = "";         // file name inside Resources/Images
    public string ContactService { get; set; } = "Graphic Design";
    public List<string> Highlights { get; set; } = new();   // tools / tags (only the featured project has them)

    // True when there is something to list (the XAML uses it to hide an empty list)
    public bool HasHighlights => Highlights.Count > 0;
}

/// <summary>One client review on the Testimonials page (testimonials.html).</summary>
public class TestimonialItem
{
    public string Quote { get; set; } = "";
    public string Name { get; set; } = "";
    public string Role { get; set; } = "";
    public string AvatarHex { get; set; } = "#3E6690";       // colour of the round initials circle
    public string Stars { get; set; } = "★★★★★";

    // The XAML binds to this: a real Color made from AvatarHex
    public Color AvatarColor => Color.FromArgb(AvatarHex);

    public string Initials =>
        string.Concat(Name.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                          .Take(2)
                          .Select(p => char.ToUpper(p[0])));
}

/// <summary>One person on the Team page (team.html).</summary>
public class TeamMember
{
    public string Name { get; set; } = "";
    public string Role { get; set; } = "";
    public string Email { get; set; } = "";

    public string Initials =>
        string.Concat(Name.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                          .Take(2)
                          .Select(p => char.ToUpper(p[0])));
}

/// <summary>A question and answer for the "Quick answers" box (copied from the website's chat assistant).</summary>
public class FaqItem
{
    public string Question { get; set; } = "";
    public string Answer { get; set; } = "";
}

/// <summary>A row on the "More" page.</summary>
public class MenuLink
{
    public string Icon { get; set; } = "";          // image file name in Resources/Images
    public string Title { get; set; } = "";
    public string Subtitle { get; set; } = "";
    public string Route { get; set; } = "";         // where tapping it goes
}

/// <summary>
/// Small shared memory of the app, used to pass information from one page to another.
/// </summary>
public static class AppState
{
    // Set when the user taps "Request this service" / "Request a similar design".
    // The Contact page picks it up, selects that service in its drop-down, and clears it.
    public static string? PendingService { get; set; }

    // The latest studio information (email, phone, social links ...). ApiService keeps it up to date
    // so the footer on every page can show the same details.
    public static StudioInfo? Studio { get; set; }
}

/// <summary>
/// All the written content of the website, in one place (copied from the website's pages).
/// If the website text changes, change it here and every page in the app follows.
/// </summary>
public static class SiteContent
{
    // ---------------- Contact details (the website footer) ----------------
    public const string FooterBlurb =
        "Crafting bold brands and creative experiences for South African businesses through professional graphic design, branding and digital solutions.";
    public const string Phone = "+27 67 790 4428";
    public const string Email = "andiswasbusinessconsultations@outlook.com";
    public const string Address = "2 Belmont Terrace, Port Elizabeth, 6001";
    public const string Hours = "Monday – Friday, 08:00 – 18:00";
    public const string Copyright = "© 2026 Andiswa Business Consultation. All Rights Reserved.";
    public const string Crafted = "Crafted with care in South Africa";

    // ---------------- About page (about.html) ----------------
    public const string Mission = "Create design that gives businesses an unfair competitive advantage.";
    public const string Vision = "Become the most recognised creative studio across the African continent.";

    public static readonly List<string> Values = new()
    {
        "Bold Thinking", "Pixel Perfect", "On-Brand Always", "South African", "Client-First"
    };

    // ---------------- Newsletter box (about.html and services.html) ----------------
    public const string NewsletterTitle = "Design Inspiration, Delivered Weekly";
    public const string NewsletterText =
        "Join 4,000+ creatives and business owners who get our weekly roundup of design trends, tips, and studio work.";

    // ---------------- Services page (services.html) ----------------
    public static readonly List<ServiceItem> Services = new()
    {
        new() { Icon = "✒️", Title = "Logo Design", Description = "Timeless marks that define brands at a glance.", ContactService = "Branding & Identity" },
        new() { Icon = "🗂️", Title = "Branding & Identity", Description = "Full visual systems — color, type, tone, touch.", ContactService = "Branding & Identity" },
        new() { Icon = "🎨", Title = "Graphic Design", Description = "Bold print and digital visuals that stop the scroll.", ContactService = "Graphic Design" },
        new() { Icon = "📄", Title = "Flyer Design", Description = "Attention-grabbing promotional materials that convert.", ContactService = "Graphic Design" },
        new() { Icon = "🖼️", Title = "Poster Design", Description = "Large-format art that commands a room.", ContactService = "Graphic Design" },
        new() { Icon = "📷", Title = "Social Media Design", Description = "Cohesive feeds and content templates that grow audiences.", ContactService = "Social Media Design" },
        new() { Icon = "🖥️", Title = "Website Design", Description = "Pixel-perfect UI/UX for brands that live online.", ContactService = "Web Design" },
        new() { Icon = "💳", Title = "Business Cards", Description = "First impressions that people actually keep.", ContactService = "Branding & Identity" },
        new() { Icon = "📖", Title = "Brochures", Description = "Narrative design that sells your story.", ContactService = "Graphic Design" },
        new() { Icon = "📘", Title = "Business Plans, Profiles & Proposals", Description = "Professionally written business plans, company profiles, and winning proposals.", ContactService = "Business Consultation" },
    };

    // The first six services, shown as a preview on the Home page
    public static readonly List<ServiceItem> HomeServices = Services.Take(6).ToList();

    public static readonly List<ReasonItem> Reasons = new()
    {
        new() { Title = "Creative & Unique", Description = "Every design is custom-crafted — no templates, no shortcuts." },
        new() { Title = "Fast Turnaround", Description = "Standard deliveries in 3–5 days. Rush options available." },
        new() { Title = "Affordable Pricing", Description = "Agency-quality creative at SME-friendly rates." },
        new() { Title = "Professional Quality", Description = "Print-ready, screen-ready files with pixel-perfect precision." },
        new() { Title = "Excellent Support", Description = "Dedicated account manager from brief to final file." },
        new() { Title = "Unlimited Creativity", Description = "Revisions until you love it — always included." },
    };

    // ---------------- Portfolio page (projects.html) ----------------
    // Software / client lines shown on every project's detail page (same as the website's pop-up)
    public const string ProjectClient = "Andiswa Business Consultation";
    public const string ProjectSoftware = "Adobe Photoshop, Illustrator, Canva";
    public const string ProjectCategory = "Graphic Design";

    public static readonly ProjectItem FeaturedProject = new()
    {
        Title = "NACWO Business Card",
        Category = "Branding",
        Filter = "branding",
        Blurb = "Professional corporate identity.",
        Description = "A modern business card created for NACWO featuring a clean layout, professional typography and a memorable brand identity.",
        Image = "nacwo_business_card.jpg",
        ContactService = "Branding & Identity",
        Highlights = new() { "Adobe Illustrator", "Brand Identity", "Print Ready" },
    };

    public static readonly List<ProjectItem> Projects = new()
    {
        FeaturedProject,
        new() { Title = "Summer Sale Flyer", Category = "Flyers", Filter = "flyers", Blurb = "Ubuhle bakho pop-up sale flyer.", Description = "Ubuhle bakho pop-up sale flyer.", Image = "sale_flyer.jpg" },
        new() { Title = "DJ Event Poster", Category = "Posters", Filter = "posters", Blurb = "Music event promotion.", Description = "Music event promotion.", Image = "dj_poster.jpg" },
        new() { Title = "Hearbalife Card", Category = "Social Media", Filter = "social", Blurb = "Hearbalife Distributor Card.", Description = "Hearbalife Distributor Card.", Image = "hearbalife_distributor.jpg", ContactService = "Social Media Design" },
        new() { Title = "Author Card", Category = "Branding", Filter = "branding", Blurb = "Lerato Molea Author Card.", Description = "Lerato Molea Author Card.", Image = "lerato_molea_author_card.jpg", ContactService = "Branding & Identity" },
        new() { Title = "Birthday Flyer", Category = "Flyers", Filter = "flyers", Blurb = "Sphokuhle 21st Birthday Invite.", Description = "Sphokuhle 21st Birthday Invite.", Image = "birthday_flyer.jpg" },
        new() { Title = "Vukauzenzele Car Wash", Category = "Posters", Filter = "posters", Blurb = "Vukauzenzele Services Poster", Description = "Vukauzenzele Services Poster", Image = "vukauzenzele_services_poster.jpg" },
        new() { Title = "CCTV Flyer", Category = "Flyers", Filter = "flyers", Blurb = "Midtown Dream CCTV Flyer.", Description = "Midtown Dream CCTV Flyer.", Image = "midtown_dream_cctv_flyer.jpg" },
        new() { Title = "Nga-Mali Card", Category = "Branding", Filter = "branding", Blurb = "Nga mali Brand Identity Card.", Description = "Nga mali Brand Identity Card.", Image = "nga_mali_brand_identity_card.jpg", ContactService = "Branding & Identity" },
        new() { Title = "Business Card", Category = "Branding", Filter = "branding", Blurb = "vukauzenzele Business card.", Description = "vukauzenzele Business card.", Image = "vukauzenzele_business_card.jpg", ContactService = "Branding & Identity" },
    };

    // ---------------- Testimonials page (testimonials.html) ----------------
    public static readonly List<TestimonialItem> Testimonials = new()
    {
        new() { Name = "Naledi Khumalo", Role = "CEO, Bloom Skincare", AvatarHex = "#5B6472",
                Quote = "They transformed our brand from forgettable to unforgettable. The logo they designed is now recognised across Johannesburg. Truly world-class creative work at a price that made sense for our stage." },
        new() { Name = "Thabo Mokoena", Role = "Founder, Vukauzenzele Services", AvatarHex = "#3E6690",
                Quote = "From our first call to the final handover, the process felt effortless. They understood exactly what our brand needed to say and said it better than we could have imagined." },
        new() { Name = "Lerato Molea", Role = "Author & Entrepreneur", AvatarHex = "#8891A0",
                Quote = "Professional, fast and genuinely creative. Our flyer designs increased foot traffic to our pop-up within the first week. We keep coming back for every campaign." },
        new() { Name = "Sphokuhle Dlamini", Role = "Director, NACWO", AvatarHex = "#1B2A41",
                Quote = "A team that actually listens. They took a vague idea and turned it into a business card and identity that gets compliments at every meeting." },
    };

    // The first testimonial, shown as a preview on the Home page
    public static readonly List<TestimonialItem> HomeTestimonials = Testimonials.Take(1).ToList();

    // ---------------- Team page (team.html) ----------------
    public static readonly List<TeamMember> Team = new()
    {
        new() { Name = "Andiswa Phewa", Role = "Founder & Creative Director", Email = "andiswa@andiswaconsultation.co.za" },
    };

    // ---------------- Quick answers (from the website's chat assistant) ----------------
    public static readonly List<FaqItem> Faq = new()
    {
        new() { Question = "How much do your services cost?",
                Answer = "Pricing depends on the scope of the project, so there isn't a single fixed rate. The quickest way to get an accurate quote is the form on this page — tell us a bit about what you need and someone from the team will follow up within a business day." },
        new() { Question = "What services do you offer?",
                Answer = "We work across brand identity, web & digital design, print, marketing strategy, social content and business consultation. See examples in the Portfolio, or tell us what you're working on and we'll point you to the right person." },
        new() { Question = "When are you open?",
                Answer = "We're open Monday to Friday, 08:00 – 18:00. You're welcome to leave a message any time and we'll reply the next business day." },
        new() { Question = "Where are you based?",
                Answer = "We're based at 2 Belmont Terrace, Port Elizabeth, 6001." },
    };

    // ---------------- "More" page rows ----------------
    public static readonly List<MenuLink> MoreLinks = new()
    {
        new() { Icon = "menu_about.png", Title = "About Us", Subtitle = "Our story, mission and vision", Route = "about" },
        new() { Icon = "menu_team.png", Title = "Team", Subtitle = "The people behind every brand we build", Route = "team" },
        new() { Icon = "menu_testimonials.png", Title = "Testimonials", Subtitle = "What our clients say about us", Route = "testimonials" },
    };
}
