using Common.Controls.Models;

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

    [JsonPropertyName("baseDevicePath")]
    public string BaseDevicePath { get; set; } = string.Empty;

    [JsonPropertyName("updatedAt")]
    public DateTime EditedAt { get; set; }

    [JsonIgnore]
    public List<Control> Controls { get; set; } = new();

    [JsonIgnore]
    public bool SuccessfullControlsFetch { get; set; } = true;

    public DeviceDTO Dto() => new()
    {
        BaseDevicePath = BaseDevicePath,
        BrokerId = BrokerId,
        Icon = Icon,
        Name = Name,
        Placing = Placing
    };

    public Device Copy() => new()
    {
        BaseDevicePath = BaseDevicePath,
        BrokerId = BrokerId,
        EditedAt = EditedAt,
        Icon = Icon.Copy(),
        Id = Id,
        Name = Name,
        Placing = Placing
    };
}
