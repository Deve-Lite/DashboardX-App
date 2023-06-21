namespace DashboardXModels.Brokers;

public class Broker
{
    [Key]
    public string BrokerId { get; set; } = string.Empty;
    public long EditedAtTicks { get; set; }
    public long CreatedAtTicks { get; set; }

    public string Name { get; set; } = string.Empty;
    public int Port { get; set; }
    public string Server { get; set; } = string.Empty;
    public bool IsSSL { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ClientID { get; set; } = string.Empty;
    public int KeepAlive { get; set; }

    public IEnumerable<Device> Devices { get; set; } = new List<Device>();
    public string UserId { get; set; } = string.Empty;
}
