

namespace Shared.Models.Brokers;

public class Broker : BaseModel
{
    public string UserId { get; set; } = string.Empty;

    public string Icon { get; set; } = string.Empty;

    [Required, StringLength(30, MinimumLength = 3)]
    public string Name { get; set; } = string.Empty;
    [Required]
    public int Port { get; set; }
    [Required]
    public string Server { get; set; } = string.Empty;

    public bool IsSSL { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;

    [Required]
    public string ClientId { get; set; } = string.Empty;
    public int KeepAlive { get; set; } = 90;

    public DateTime EditedAt { get; set; }

    //TODO: Uncoment when devices will be implemented
    //public IEnumerable<Device> Devices { get; set; } = new List<Device>();
}
