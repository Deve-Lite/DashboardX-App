

namespace Shared.Models.Brokers;

public class Broker : BaseModel
{
    [JsonPropertyName("icon")]
    public string Icon { get; set; } = string.Empty;
    [JsonPropertyName("iconBackgroundColor")]
    public string IconBackgroundColor { get; set; } = string.Empty;
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
    [JsonPropertyName("port")]
    public int Port { get; set; }
    [JsonPropertyName("server")]
    public string Server { get; set; } = string.Empty;

    [JsonPropertyName("isSsl")]
    public bool IsSSL { get; set; }
    [JsonPropertyName("sslUsername")]
    public string Username { get; set; } = string.Empty;
    [JsonPropertyName("sslPassword")]
    public string Password { get; set; } = string.Empty;

    [JsonPropertyName("clientId")]
    public string ClientId { get; set; } = string.Empty;
    [JsonPropertyName("keepAlive")]
    public int KeepAlive { get; set; } = 90;

    [JsonPropertyName("updatedAt")]
    public DateTime EditedAt { get; set; }

    public Broker Copy() => new()
    {
        ClientId = ClientId,
        EditedAt = EditedAt,
        Icon = Icon,
        Id = Id,
        IsSSL = IsSSL,
        KeepAlive = KeepAlive,
        Name = Name,
        Password = Password,
        Port = Port,
        Server = Server,
        Username = Username
    };

}
