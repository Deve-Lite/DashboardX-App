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

    public Control(string newId, ControlDTO dto)
    {
        Id = newId;
        DeviceId = dto.DeviceId;
        Name = dto.Name;
        Icon = dto.Icon.Copy();
        Topic = dto.Topic;
        Type = dto.Type;
        QualityOfService = dto.QualityOfService;
        DisplayName = dto.DisplayName;
        IsAvailable = dto.IsAvailable;
        IsConfiramtionRequired = dto.IsConfiramtionRequired;
        NotifyOnPublish = dto.NotifyOnPublish;
        Attributes = dto.Attributes.Copy();
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

    public bool IsSubscribed { get; set; } = false;

    public ControlDTO Dto() => new ControlDTO
    {
        Id = Id,
        DeviceId = DeviceId,
        Name = Name,
        Icon = Icon.Copy(),
        Topic = Topic,
        Type = Type,
        QualityOfService = QualityOfService,
        DisplayName = DisplayName,
        IsAvailable = IsAvailable,
        IsConfiramtionRequired = IsConfiramtionRequired,
        NotifyOnPublish = NotifyOnPublish,
        Attributes = Attributes.Copy(),
    };

    public bool ShouldBeSubscribed()
    {
        return Type == ControlType.Color  ||
               Type == ControlType.Slider ||
               Type == ControlType.State  ||
               Type == ControlType.Switch ||
               Type == ControlType.Radio  || 
               Type == ControlType.Text;
    }

    public Control Copy()
    {
        var attrCopy = Attributes.Copy();

        return new()
        {
            Id = Id,
            DeviceId = DeviceId,
            Name = Name,
            Icon = Icon.Copy(),
            Topic = Topic,
            Type = Type,
            QualityOfService = QualityOfService,
            DisplayName = DisplayName,
            IsAvailable = IsAvailable,
            IsConfiramtionRequired = IsConfiramtionRequired,
            NotifyOnPublish = NotifyOnPublish,
            Attributes = attrCopy,
        };
    }

    public void Update(Control newControl)
    {
        Id = newControl.Id;
        DeviceId = newControl.DeviceId;
        Name = newControl.Name;
        Icon = newControl.Icon.Copy();
        Topic = newControl.Topic;
        Type = newControl.Type;
        QualityOfService = newControl.QualityOfService;
        DisplayName = newControl.DisplayName;
        IsAvailable = newControl.IsAvailable;
        IsConfiramtionRequired = newControl.IsConfiramtionRequired;
        NotifyOnPublish = newControl.NotifyOnPublish;
        Attributes = newControl.Attributes.Copy();
    }
}