
namespace Shared.Models.Auth;

public class Tokens
{
    [JsonPropertyName("accessToken")]
    public string AccessToken { get; set; } = string.Empty;
    [JsonPropertyName("refreshToken")]
    public string RefreshToken { get; set; } = string.Empty;
}
