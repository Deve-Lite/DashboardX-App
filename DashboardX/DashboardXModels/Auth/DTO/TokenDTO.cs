
namespace DashboardXModels.Auth.DTO;

public class TokenDTO
{
    [JsonPropertyName("accessToken")]
    public string AccessToken { get; set; } = string.Empty;
    [JsonPropertyName("refreshToken")]
    public string RefreshToken { get; set; } = string.Empty;
}
