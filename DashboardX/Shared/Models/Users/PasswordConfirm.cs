
namespace Shared.Models.Users;

public class PasswordConfirm
{
    [JsonPropertyName("password")]
    public string Password { get; set; } = string.Empty;

}
