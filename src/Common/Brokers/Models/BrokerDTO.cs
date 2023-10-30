namespace Common.Brokers.Models;

public class BrokerDTO : BaseModel
{
    [JsonPropertyName("icon")]
    public Icon Icon { get; set; } = new();
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
    [JsonPropertyName("port")]
    public int Port { get; set; }
    [JsonPropertyName("server")]
    public string Server { get; set; } = string.Empty;

    [JsonPropertyName("isSsl")]
    public bool IsSSL { get; set; }

    [JsonPropertyName("clientId")]
    public string ClientId { get; set; } = string.Empty;
    [JsonPropertyName("keepAlive")]
    public int KeepAlive { get; set; } = 90;

    [JsonPropertyName("updatedAt")]
    public DateTime EditedAt { get; set; }
}



