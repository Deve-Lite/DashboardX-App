

namespace Shared.Models.Brokers;

public class Broker : BaseModel
{
    [JsonPropertyName("icon")]
    public string Icon { get; set; } = string.Empty;

    [JsonPropertyName("name"), Required, StringLength(30, MinimumLength = 3)]
    public string Name { get; set; } = string.Empty;
    [JsonPropertyName("port"), Required]
    public int Port { get; set; }
    [JsonPropertyName("server"), Required]
    public string Server { get; set; } = string.Empty;

    [JsonPropertyName("isSsl")]
    public bool IsSSL { get; set; }
    [JsonPropertyName("sslUsername")]
    public string Username { get; set; } = string.Empty;
    [JsonPropertyName("sslPassword")]
    public string Password { get; set; } = string.Empty;

    [JsonPropertyName("clientId"), Required]
    public string ClientId { get; set; } = string.Empty;
    [JsonPropertyName("keepAlive")]
    public int KeepAlive { get; set; } = 90;

    [JsonPropertyName("updatedAt")]
    public DateTime EditedAt { get; set; }

}
