using MQTTnet.Protocol;

namespace Common.Controls.Models;

public class Control : BaseModel
{
    #region Constructors

    public Control()
    {
        Type = ControlType.Button;
    }

    public Control(string deviceId)
    {
        DeviceId = deviceId;
        Type = ControlType.Button;
    }

    #endregion

    #region Foreign Keys

    [JsonPropertyName("deviceId")]
    public string DeviceId { get; set; } = string.Empty;

    #endregion

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
    [JsonPropertyName("icon")]
    public string Icon { get; set; } = string.Empty;
    [JsonPropertyName("iconBackgroundColor")]
    public string IconBackgroundColor { get; set; } = string.Empty;
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

    [JsonPropertyName("displayName")]
    public bool DisplayName { get; set; }
    [JsonPropertyName("isAvailable")]
    public bool IsAvailable { get; set; }
    [JsonPropertyName("isConfirmationRequired")]
    public bool IsConfiramtionRequired { get; set; }
    [JsonPropertyName("notifyOnPublish")]
    public bool NotifyOnPublish { get; set; }

    [JsonPropertyName("attributes")]
    public ControlAttributes Attributes { get; set; } = new();

    #region Methods

    public static Control Create(Control dto)
    {
        return new Control
        {
            Id = dto.Id,
            QualityOfService = dto.QualityOfService,
            Type = dto.Type,
            Topic = dto.Topic,
            DeviceId = dto.DeviceId,
            StringType = dto.StringType,
            Icon = dto.Icon,
            IconBackgroundColor = dto.IconBackgroundColor,
            IsAvailable = dto.IsAvailable,
            IsConfiramtionRequired = dto.IsConfiramtionRequired,
        };
    }

    public bool IsTheSame(Control control)
    {
        //TODO compare attruibutes
        return Id == control.Id &&
               DeviceId == control.DeviceId &&
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
            Name = Name,
            Id = Id,
            DeviceId = DeviceId,
            StringType = StringType,
            Icon = Icon,
            IconBackgroundColor = IconBackgroundColor,
            IsAvailable = IsAvailable,
            IsConfiramtionRequired = IsConfiramtionRequired,
            QualityOfService = QualityOfService,
            Topic = Topic,
            Attributes = Attributes.Copy(),
        };
    }

    #endregion
}
