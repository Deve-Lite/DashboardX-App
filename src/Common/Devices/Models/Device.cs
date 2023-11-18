namespace Common.Devices.Models;

public class Device : BaseModel
{
    public string Name { get; set; } = string.Empty;
    public string Placing { get; set; } = string.Empty;
    public string BrokerId { get; set; } = string.Empty;
    public string BaseDevicePath { get; set; } = string.Empty;
    public Icon Icon { get; set; } = new();
    public DateTime EditedAt { get; set; }
    public bool SuccessfullControlsDownload { get; set; } = false;

    public static Device FromDto(DeviceDTO x) => new()
    {
            Id = x.Id,
            Name = x.Name,
            Placing = x.Placing,
            Icon = x.Icon,
            BrokerId = x.BrokerId,
            BaseDevicePath = x.BaseDevicePath,
            EditedAt = x.EditedAt
    };

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
