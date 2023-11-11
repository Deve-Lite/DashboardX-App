namespace Common.Auth.Models;

public class ResendConfirmEmailModel
{
    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;
}
