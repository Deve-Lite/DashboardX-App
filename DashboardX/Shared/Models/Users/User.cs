
namespace Shared.Models.Users;

public class User
{
    [JsonPropertyName("id")]
    public string UserId { get; set; } = string.Empty;
    [JsonPropertyName("name")]
    public string Username { get; set; } = string.Empty;
    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;
    [JsonPropertyName("isAdmin")]
    public bool IsAdmin { get; set; } = false;

    [JsonPropertyName("theme")]
    public string StringTheme { get; set; } = "inherit";
    [JsonIgnore]
    public Theme Theme 
    {
        get => StringTheme switch
        {
            "light" => Theme.Light,
            "dark" => Theme.Dark,
            _ => Theme.Inherit
        };
        
        set => StringTheme = value switch
        {
            Theme.Light => "light",
            Theme.Dark => "dark",
            _ => "inherit"
        };
        
    }

    
    [JsonPropertyName("language")]
    public string StringLanguage { get; set; } = "en";
    [JsonIgnore]
    public Language Language 
    {
        get => StringLanguage switch
        {
            "pl" => Language.Polish,
            _ => Language.English
        };

        set => StringLanguage = value switch
        {
            Language.Polish => "pl",
            _ => "en"
        };
    }

    
    // TODO: NEW FIELD
    [JsonPropertyName("avatar")]
    public string Avatar { get; set; } = string.Empty;

    public Preferences GetPreferences() => new()
    {
        Language = Language,
        Theme = Theme
    };
    
}
