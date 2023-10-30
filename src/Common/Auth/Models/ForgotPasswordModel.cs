namespace Common.Auth.Models;

public class ForgetPasswordModel
{
    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;
}

