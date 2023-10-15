
namespace Common.Brokers.Models;

public record BrokerCredentialsDTO
{
    [JsonPropertyName("username")]
    public string? Username { get; set; }
    [JsonPropertyName("password")]
    public string? Password { get; set; }
}
