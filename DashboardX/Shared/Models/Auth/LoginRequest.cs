
namespace Shared.Models.Auth;

public class LoginRequest
{
    [JsonPropertyName("email"), Required, MinLength(6), MaxLength(256), EmailAddress]
    public string Email { get; set; } = string.Empty;
    [JsonPropertyName("password"), Required, MinLength(6), MaxLength(30)]
    public string Password { get; set; } = string.Empty;
}
