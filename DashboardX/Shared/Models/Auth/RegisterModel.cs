namespace Shared.Models.Auth;

public class RegisterModel
{
    [JsonPropertyName("username"), Required, MinLength(3), MaxLength(30)]
    public string Username { get; set; } = string.Empty;

    [JsonPropertyName("password"), Required, MinLength(6), MaxLength(30),
        RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$", ErrorMessage = "The password must contain at least one lowercase letter, one uppercase letter, and one digit.")]
    public string Password { get; set; } = string.Empty;

    [ Required, MinLength(6), MaxLength(30), Compare(nameof(Password), ErrorMessage = "Passwords do not match!")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [JsonPropertyName("email"), Required, MinLength(6), MaxLength(256), EmailAddress]
    public string Email { get; set; } = string.Empty;
}
