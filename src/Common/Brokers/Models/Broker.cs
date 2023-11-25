namespace Common.Brokers.Models;

public class Broker : BaseModel
{
    public Icon Icon { get; set; } = new();
    public string Name { get; set; } = string.Empty;
    public int Port { get; set; }
    public string Server { get; set; } = string.Empty;
    public bool SSL { get; set; }
    public string ClientId { get; set; } = string.Empty;
    public int KeepAlive { get; set; } = 90;
    public DateTime EditedAt { get; set; }

    public static Broker FromDto(BrokerDTO dto) => new()
    {
        ClientId = dto.ClientId,
        EditedAt = dto.EditedAt,
        Icon = dto.Icon.Copy(),
        Id = dto.Id,
        SSL = dto.IsSSL,
        KeepAlive = dto.KeepAlive,
        Name = dto.Name,
        Port = dto.Port,
        Server = dto.Server,
    };

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
