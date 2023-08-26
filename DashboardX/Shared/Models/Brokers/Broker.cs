

namespace Shared.Models.Brokers;

public class Broker : BaseModel
{
    [JsonPropertyName("icon")]
    public string Icon { get; set; } = string.Empty;

    [JsonPropertyName("name"), Required, MinLength(2), MaxLength(64)]
    public string Name { get; set; } = string.Empty;
    [JsonPropertyName("port"), Required]
    public int Port { get; set; }
    [JsonPropertyName("server"), Required, MinLength(2), MaxLength(256)]
    public string Server { get; set; } = string.Empty;

    [JsonPropertyName("isSsl")]
    public bool IsSSL { get; set; }
    [JsonPropertyName("sslUsername"), MaxLength(64)]
    public string Username { get; set; } = string.Empty;
    [JsonPropertyName("sslPassword"), MaxLength(64)]
    public string Password { get; set; } = string.Empty;

    [JsonPropertyName("clientId"), Required, MaxLength(64)]
    public string ClientId { get; set; } = string.Empty;
    [JsonPropertyName("keepAlive"), Required]
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
