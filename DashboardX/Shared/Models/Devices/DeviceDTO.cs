using Shared.Models.Controls;

namespace Shared.Models.Devices;

public  class DeviceDTO
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("placing")]
    public string Placing { get; set; } = string.Empty;

    [JsonPropertyName("icon")]
    public string Icon { get; set; } = "default.png";

    [JsonPropertyName("iconBackgroundColor")]
    public string IconBackgroundColor { get; set; } = string.Empty;

    [JsonPropertyName("brokerId")]
    public string BrokerId { get; set; } = string.Empty;

    [JsonPropertyName("baseDevicePath")]
    public string BaseDevicePath { get; set; } = string.Empty;

}
