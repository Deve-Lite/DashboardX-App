namespace Common.Users.Models;

public class PasswordConfirm
{
    [JsonPropertyName("password")]
    public string Password { get; set; } = string.Empty;

}
