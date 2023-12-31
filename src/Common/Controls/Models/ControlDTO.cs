using MQTTnet.Protocol;

namespace Common.Controls.Models;

public class ControlDto : BaseModel
{
    public ControlDto()
    {
        Type = ControlType.Text;
    }

    public ControlDto(ControlModel control, ControlAttributesModel attributes)
    {
        Id = control.Id;
        DeviceId = control.DeviceId;
        Name = control.Name;
        Icon = control.Icon.Copy();
        Topic = control.Topic;
        Type = control.Type;
        QualityOfService = control.QualityOfService;
        DisplayName = control.DisplayName;
        IsAvailable = control.IsAvailable;
        IsConfiramtionRequired = control.IsConfiramtionRequired;
        NotifyOnPublish = control.NotifyOnPublish;
        Attributes = attributes.Attributes();
    }

    [JsonPropertyName("deviceId")]
    public string DeviceId { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("icon")]
    public Icon Icon { get; set; } = new();

    [JsonPropertyName("topic")]
    public string Topic { get; set; } = string.Empty;

    [JsonPropertyName("type")]
    public string StringType { get; set; } = string.Empty;
    [JsonIgnore]
    public ControlType Type
    {
        get
        {
            return StringType switch
            {
                "button" => ControlType.Button,
                "color" => ControlType.Color,
                "date-time" => ControlType.DateTime,
                "radio" => ControlType.Radio,
                "slider" => ControlType.Slider,
                "state" => ControlType.State,
                "switch" => ControlType.Switch,
                "text-out" => ControlType.Text,
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
                ControlType.Radio => "radio",
                ControlType.Slider => "slider",
                ControlType.State => "state",
                ControlType.Switch => "switch",
                ControlType.Text => "text-out",
                _ => "button",
            };
        }
    }

    [JsonPropertyName("qualityOfService")]
    public MqttQualityOfServiceLevel QualityOfService { get; set; } = MqttQualityOfServiceLevel.AtMostOnce;

    [JsonPropertyName("canDisplayName")]
    public bool DisplayName { get; set; } = true;

    [JsonPropertyName("isAvailable")]
    public bool IsAvailable { get; set; } = true;

    [JsonPropertyName("isConfirmationRequired")]
    public bool IsConfiramtionRequired { get; set; } = false;

    [JsonPropertyName("canNotifyOnPublish")]
    public bool NotifyOnPublish { get; set; } = false;

    [JsonPropertyName("attributes")]
    public ControlAttributesDto Attributes { get; set; } = new();
}
