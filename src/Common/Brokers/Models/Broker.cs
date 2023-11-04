﻿namespace Common.Brokers.Models;

//TODO: Create DTO for broker

public class Broker : BaseModel
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
    public bool SSL { get; set; }

    [JsonPropertyName("clientId")]
    public string ClientId { get; set; } = string.Empty;
    [JsonPropertyName("keepAlive")]
    public int KeepAlive { get; set; } = 90;

    [JsonPropertyName("updatedAt")]
    public DateTime EditedAt { get; set; }

    public BrokerDTO Dto() => new()
    {
        ClientId = ClientId,
        EditedAt = EditedAt,
        Icon = Icon.Copy(),
        Id = Id,
        IsSSL = SSL,
        KeepAlive = KeepAlive,
        Name = Name,
        Port = Port,
        Server = Server,
    };

    public void Update(Broker broker)
    {
        ClientId = broker.ClientId;
        EditedAt = broker.EditedAt;
        Icon = broker.Icon;
        SSL = broker.SSL;
        KeepAlive = broker.KeepAlive;
        Name = broker.Name;
        Port = broker.Port;
        Server = broker.Server;
    }
}
