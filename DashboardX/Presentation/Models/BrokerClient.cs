using Infrastructure;
using MQTTnet.Client;
using Shared.Models.Brokers;

namespace Presentation.Models;

public class BrokerClient
{
    public Broker Broker { get; set; } = new Broker();
    public IMqttClient? MqttService { get; set; }
    public Result? Result { get; set; }
}
