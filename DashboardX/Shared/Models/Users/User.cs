
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
    public string AppTheme { get; set; } = string.Empty;
    [JsonPropertyName("language")]
    public string Language { get; set; } = string.Empty;
    
    // TODO: NEW FIELD
    [JsonPropertyName("avatar")]
    public string Avatar { get; set; } = string.Empty;
}
