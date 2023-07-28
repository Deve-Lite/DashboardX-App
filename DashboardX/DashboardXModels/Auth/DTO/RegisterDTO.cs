
namespace DashboardXModels.Auth.DTO;

public class RegisterDTO
{
    [JsonPropertyName("name")]
    public string Username { get; set; } = string.Empty;
    [JsonPropertyName("password")]
    public string Password { get; set; } = string.Empty;
    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;
}
