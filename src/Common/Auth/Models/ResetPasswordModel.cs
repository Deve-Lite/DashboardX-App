namespace Common.Auth.Models;

public class ResetPasswordModel
	{
    [JsonPropertyName("password")]
    public string Password { get; set; } = string.Empty;

    [JsonIgnore]
    public string ConfirmPassword { get; set; } = string.Empty;
}

