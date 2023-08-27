using MQTTnet.Protocol;

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
    public MqttQualityOfServiceLevel QualityOfService { get; set; }
    [JsonPropertyName("topic")]
    public string Topic { get; set; } = string.Empty;

    public bool IsTheSame(Control control)
    {
        return DeviceId == control.DeviceId &&
               Type == control.Type &&
               Icon == control.Icon &&
               IconBackgroundColor == control.IconBackgroundColor &&
               IsAvailable == control.IsAvailable &&
               IsConfiramtionRequired == control.IsConfiramtionRequired &&
               QualityOfService == control.QualityOfService &&
               Topic == control.Topic;
    }
}
