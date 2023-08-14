using Infrastructure;
using MQTTnet.Client;
using Shared.Models.Brokers;
using Shared.Models.Devices;

namespace Presentation.Models;

public class DeviceClient
{
    public Device Device { get; set; } = new Device();
    public IMqttClient? MqttService { get; set; }
    public Broker Broker { get; set; } = new Broker();
    public Result? Result { get; set; }
}
