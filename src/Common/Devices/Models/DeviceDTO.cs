namespace Common.Devices.Models;

public class DeviceDTO : BaseModel
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
}
