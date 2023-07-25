using DashboardXModels.Brokers;
using MQTTnet;
using MQTTnet.Client;
using System.Text;

namespace DashboardXModels;

/// <summary>
/// In case of any broker update create new InitializedBroker and replace old one.
/// </summary>
public class InitializedBroker : IDisposable
{
    public IDictionary<string, Device> Devices { get; private set; }
    public IMqttClient Client { get; private set; }
    public Broker Broker { get; private set; }

    public string Id { get => Broker.BrokerId; }

    public bool NotAvailable { get => Broker.BrokerId == null; }

    public InitializedBroker(Broker broker, IMqttClient client)
    {
        Client = client;
        Broker = broker;
        Devices = new Dictionary<string, Device>();
    }

    public InitializedBroker()
    {
        Devices = new Dictionary<string, Device>();
        Broker = new Broker();
        Client = new MqttFactory().CreateMqttClient();
    }

    public void UpdateClient(IMqttClient client)
    {
        Client = client;
    }

    public void Dispose() => Client.Dispose();
}
