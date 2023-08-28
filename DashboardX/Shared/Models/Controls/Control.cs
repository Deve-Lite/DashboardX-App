using MQTTnet.Protocol;

namespace Shared.Models.Controls;

public class Control : BaseModel
{
    [JsonPropertyName("deviceId")]
    public string DeviceId { get; set; } = string.Empty;

    public ControlTypes Type
    {
        get
        {
            return StringType switch
            {
                "button" => ControlTypes.Button,
                "color" => ControlTypes.Color,
                "date-time" => ControlTypes.DateTime,
                "multi-button" => ControlTypes.MultiButton,
                "radio" => ControlTypes.Radio,
                "slider" => ControlTypes.Slider,
                "state" => ControlTypes.State,
                "switch" => ControlTypes.Switch,
                "text-out" => ControlTypes.TextOut,
                _ => ControlTypes.Button,
            };
        }
        set
        {
            StringType = value switch
            {
                ControlTypes.Button => "button",
                ControlTypes.Color => "color",
                ControlTypes.DateTime => "date-time",
                ControlTypes.MultiButton => "multi-button",
                ControlTypes.Radio => "radio",
                ControlTypes.Slider => "slider",
                ControlTypes.State => "state",
                ControlTypes.Switch => "switch",
                ControlTypes.TextOut => "text-out",
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
}
