
namespace Shared.Models.Users;

public class User : Preferences
{

    [JsonPropertyName("name")]
    public string Username { get; set; } = string.Empty;
    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;
    [JsonPropertyName("isAdmin")]
    public bool IsAdmin { get; set; } = false;

    
    // TODO: NEW FIELD
    [JsonPropertyName("avatar")]
    public string Avatar { get; set; } = string.Empty;

    public Preferences GetPreferences() => new()
    {
        Language = Language,
        Theme = Theme
    };
    
}
