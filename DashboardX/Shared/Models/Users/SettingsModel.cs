
namespace Shared.Models.Users;

public class SettingsModel
{
    [JsonPropertyName("theme")]
    public string AppTheme { get; set; } = string.Empty;
    [JsonPropertyName("language")]
    public string Language { get; set; } = string.Empty;

    // TODO: NEW FIELD
    [JsonPropertyName("avatar")]
    public string Avatar { get; set; } = string.Empty;
}
