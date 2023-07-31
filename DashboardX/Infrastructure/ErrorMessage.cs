
using System.Net;
using System.Text.Json.Serialization;

namespace Infrastructure;

public class ErrorMessage
{
    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;
    [JsonPropertyName("statusCode")]
    public HttpStatusCode StatusCode { get; set; }
    [JsonPropertyName("error")]
    public string Error { get; set; } = string.Empty;
}
