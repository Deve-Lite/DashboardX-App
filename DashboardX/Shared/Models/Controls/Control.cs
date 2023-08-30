using MQTTnet.Protocol;

namespace Shared.Models.Controls;

public class Control : BaseModel
{
    [JsonPropertyName("deviceId")]
    public string DeviceId { get; set; } = string.Empty;

    public ControlType Type
    {
        get
        {
            return StringType switch
            {
                "button" => ControlType.Button,
                "color" => ControlType.Color,
                "date-time" => ControlType.DateTime,
                "multi-button" => ControlType.MultiButton,
                "radio" => ControlType.Radio,
                "slider" => ControlType.Slider,
                "state" => ControlType.State,
                "switch" => ControlType.Switch,
                "text-out" => ControlType.TextOut,
                _ => ControlType.Button,
            };
        }
        set
        {
            StringType = value switch
            {
                ControlType.Button => "button",
                ControlType.Color => "color",
                ControlType.DateTime => "date-time",
                ControlType.MultiButton => "multi-button",
                ControlType.Radio => "radio",
                ControlType.Slider => "slider",
                ControlType.State => "state",
                ControlType.Switch => "switch",
                ControlType.TextOut => "text-out",
                _ => "button",
            };
        }
    }

    [JsonPropertyName("type"), Required]
    public string StringType { get; set; } = string.Empty;
    [JsonPropertyName("icon"), Required]
    public string Icon { get; set; } = string.Empty;
    [JsonPropertyName("iconBackgroundColor"), Required]
    public string IconBackgroundColor { get; set; } = string.Empty;
    [JsonPropertyName("isAvailable"), Required]
    public bool IsAvailable { get; set; }
    [JsonPropertyName("isConfirmationRequired"), Required]
    public bool IsConfiramtionRequired { get; set; }
    [JsonPropertyName("qualityOfService"), Required]
    public MqttQualityOfServiceLevel QualityOfService { get; set; }
    [JsonPropertyName("topic"), Required, MinLength(1), MaxLength(64)]
    public string Topic { get; set; } = string.Empty;

    public bool IsTheSame(Control control)
    {
        return DeviceId == control.DeviceId &&
               StringType == control.StringType &&
               Icon == control.Icon &&
               IconBackgroundColor == control.IconBackgroundColor &&
               IsAvailable == control.IsAvailable &&
               IsConfiramtionRequired == control.IsConfiramtionRequired &&
               QualityOfService == control.QualityOfService &&
               Topic == control.Topic;
    }

    public Control Copy()
    {
        return new()
        {
            DeviceId = DeviceId,
            StringType = StringType,
            Icon = Icon,
            IconBackgroundColor = IconBackgroundColor,
            IsAvailable = IsAvailable,
            IsConfiramtionRequired = IsConfiramtionRequired,
            QualityOfService = QualityOfService,
            Topic = Topic
        };
    }
}
