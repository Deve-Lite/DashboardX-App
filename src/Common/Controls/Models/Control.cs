using MQTTnet.Protocol;

namespace Common.Controls.Models;

public class Control : BaseModel
{
    #region Constructors

    public Control()
    {
        Type = ControlType.Text;
    }

    public Control(string deviceId)
    {
        DeviceId = deviceId;
        Type = ControlType.Text;
    }

    #endregion

    #region Foreign Keys

    [JsonPropertyName("deviceId")]
    public string DeviceId { get; set; } = string.Empty;

    #endregion

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
    public ControlAttributes Attributes { get; set; } = new();

    #region Methods

    public bool IsTheSame(Control control)
    {
        return Id == control.Id &&
               DeviceId == control.DeviceId &&
               StringType == control.StringType &&
               Icon.Name == control.Icon.Name &&
               Icon.BackgroundHex == control.Icon.BackgroundHex &&
               IsAvailable == control.IsAvailable &&
               IsConfiramtionRequired == control.IsConfiramtionRequired &&
               QualityOfService == control.QualityOfService &&
               Topic == control.Topic;
    }

    public Control Copy()
    {
        var attrCopy = Attributes.Copy();

        return new()
        {
            Name = Name,
            Id = Id,
            DeviceId = DeviceId,
            StringType = StringType,
            Icon = Icon.Copy(),
            IsAvailable = IsAvailable,
            IsConfiramtionRequired = IsConfiramtionRequired,
            QualityOfService = QualityOfService,
            Topic = Topic,
            Attributes = attrCopy,
        };
    }

    public void Update(Control newControl)
    {
        Name = newControl.Name;
        Id = newControl.Id;
        DeviceId = newControl.DeviceId;
        StringType = newControl.StringType;
        Icon = newControl.Icon;
        IsAvailable = newControl.IsAvailable;
        IsConfiramtionRequired = newControl.IsConfiramtionRequired;
        QualityOfService = newControl.QualityOfService;
        Topic = newControl.Topic;
        Attributes = newControl.Attributes;
    }

    #endregion
}
