using Infrastructure;
using MQTTnet.Client;
using Shared.Models.Brokers;
using Shared.Models.Devices;

namespace Presentation.Models;

public class BrokerClient
{
    public Broker Broker { get; set; } = new();
    public IMqttClient? MqttService { get; set; }
    public Result? Result { get; set; }
    public List<Device> Devices { get; set; } = new();
}
