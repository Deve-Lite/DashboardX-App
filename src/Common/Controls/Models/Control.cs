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

    public Control(string newId, ControlDto dto)
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
        Attributes = dto.Attributes.Model();
    }

    #endregion

    #region Foreign Keys
    public string DeviceId { get; set; } = string.Empty;
    #endregion

    public string Name { get; set; } = string.Empty;
    public Icon Icon { get; set; } = new();
    public string Topic { get; set; } = string.Empty;
    public string StringType { get; set; } = string.Empty;
    public ControlType Type { get; set; }
    public MqttQualityOfServiceLevel QualityOfService { get; set; } = MqttQualityOfServiceLevel.AtMostOnce;
    public bool DisplayName { get; set; } = true;
    public bool IsAvailable { get; set; } = true;
    public bool IsConfiramtionRequired { get; set; } = false;
    public bool NotifyOnPublish { get; set; } = false;
    public ControlAttributes Attributes { get; set; } = new();

    public ControlSubscribeStatus SubscribeStatus { get; set; } = ControlSubscribeStatus.NotAttempt;

    public ControlModel FormModel() => new ControlModel
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

    public static Control FromDto(ControlDto x) => new Control
    {
        Id = x.Id,
        DeviceId = x.DeviceId,
        Name = x.Name,
        Icon = x.Icon.Copy(),
        Topic = x.Topic,
        Type = x.Type,
        QualityOfService = x.QualityOfService,
        DisplayName = x.DisplayName,
        IsAvailable = x.IsAvailable,
        IsConfiramtionRequired = x.IsConfiramtionRequired,
        NotifyOnPublish = x.NotifyOnPublish,
        Attributes = x.Attributes.Model(),
    };

    public ControlDto Dto()
    {
        return new ControlDto
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
            Attributes = Attributes.Dto(),
        };
    }
}