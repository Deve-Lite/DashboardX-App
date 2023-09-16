
namespace Shared.Models.Users;

public class ChangePasswordModel
{
    [JsonPropertyName("password")]
    public string Password { get; set; } = string.Empty;
    [JsonPropertyName("newPassword")]
    public string NewPassword { get; set; } = string.Empty;
    [JsonIgnore]
    public string ConfirmNewPassword { get; set; } = string.Empty;
}
