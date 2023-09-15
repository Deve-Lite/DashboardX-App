
namespace Shared.Models.Users;

public class NewPasswordModel
{
    [JsonPropertyName("password")]
    public string Password { get; set; } = string.Empty;
    [JsonPropertyName("newPassword")]
    public string NewPassword { get; set; } = string.Empty;
}
