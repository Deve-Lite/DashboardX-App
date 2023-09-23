namespace Core.Models;

public class ErrorMessage
{
    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;
    [JsonPropertyName("error")]
    public string Error { get; set; } = string.Empty;
}
