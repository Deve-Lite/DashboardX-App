namespace Common.Devices.Models;

public class Device : BaseModel
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("placing")]
    public string Placing { get; set; } = string.Empty;

    [JsonPropertyName("icon")]
    public Icon Icon { get; set; } = new();

    [JsonPropertyName("brokerId")]
    public string BrokerId { get; set; } = string.Empty;

    [JsonPropertyName("basePath")]
    public string BaseDevicePath { get; set; } = string.Empty;

    [JsonPropertyName("updatedAt")]
    public DateTime EditedAt { get; set; }

    [JsonIgnore]
    public bool SuccessfullControlsDownload { get; set; } = false;

    public DeviceDTO Dto() => new()
    {
        BaseDevicePath = BaseDevicePath,
        BrokerId = BrokerId,
        Icon = Icon,
        Name = Name,
        Id = Id,
        Placing = Placing
    };

    public void Update(Device device)
    {
        this.Name = device.Name;
        this.Placing = device.Placing;
        this.Icon = device.Icon;
        this.BrokerId = device.BrokerId;
        this.BaseDevicePath = device.BaseDevicePath;
        this.EditedAt = device.EditedAt;
    }
}
