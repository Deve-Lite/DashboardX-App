using DashboardXModels.Controls;

namespace Shared.Models.Controls;

public class Control : BaseModel
{
    [JsonPropertyName("deviceId")]
    public string DeviceId { get; set; } = string.Empty;
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;
    [JsonPropertyName("icon")]
    public string Icon { get; set; } = string.Empty;
    [JsonPropertyName("iconBackgroundColor")]
    public string IconBackgroundColor { get; set; } = string.Empty;
    [JsonPropertyName("isAvailable")]
    public bool IsAvailable { get; set; }
    [JsonPropertyName("isConfirmationRequired")]
    public bool IsConfiramtionRequired { get; set; }
    [JsonPropertyName("qualityOfService")]
    public QualityOfService QualityOfService { get; set; }
    [JsonPropertyName("topic")]
    public string Topic { get; set; } = string.Empty;
}
