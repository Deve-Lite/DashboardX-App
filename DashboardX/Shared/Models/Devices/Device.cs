
namespace Shared.Models.Devices;

public class Device : BaseModel
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
    [JsonPropertyName("placing")]
    public string Placing { get; set; } = string.Empty;

    [JsonPropertyName("icon")]
    public string Icon { get; set; } = string.Empty;
    [JsonPropertyName("iconBackgroundColor")]
    public string IconBackgroundColor { get; set; } = string.Empty;

    [JsonPropertyName("brokerId")]
    public string BrokerId { get; set; } = string.Empty;
    [JsonPropertyName("baseDevicePath")]
    public string BaseDevicePath { get; set; } = string.Empty;

    [JsonPropertyName("updatedAt")]
    public DateTime EditedAt { get; set; }
}
