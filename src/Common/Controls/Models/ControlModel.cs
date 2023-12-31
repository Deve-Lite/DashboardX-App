using MQTTnet.Protocol;

namespace Common.Controls.Models;

public class ControlModel : BaseModel
{
    public ControlModel()
    {
        Type = ControlType.Text;
    }

    public string DeviceId { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public Icon Icon { get; set; } = new();

    public string Topic { get; set; } = string.Empty;

    public ControlType Type { get; set; }

    public MqttQualityOfServiceLevel QualityOfService { get; set; }

    public bool DisplayName { get; set; } = true;

    public bool IsAvailable { get; set; } = true;

    public bool IsConfiramtionRequired { get; set; } = false;

    public bool NotifyOnPublish { get; set; } = false;
}
