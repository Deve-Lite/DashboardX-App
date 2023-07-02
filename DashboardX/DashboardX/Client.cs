using DashboardXModels.Brokers;
using DashboardXModels.Devices;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;
using MQTTnet;
using MQTTnet.Client;
using System.Text;

namespace DashboardX;

public class Client
{
    private readonly MqttFactory _factory;

    public Broker Broker { get; private set; }
    public IEnumerable<Device> Devices { get; private set; }
    public IMqttClient BrokerClient { get; private set; }
    public IDictionary<string, string> LastMessageOnTopic { get; private set; }
    public bool IsConnected => BrokerClient.IsConnected;

    public Action OnMessageReceived { get; set; }

    public Client(Broker broker, MqttFactory factory, IJSRuntime runtime, IEnumerable<Device> devices)
    {
        Broker = broker;
        Devices = devices;
        _factory = factory;
        BrokerClient = factory.CreateMqttClient();
        LastMessageOnTopic = new Dictionary<string, string>();

        BrokerClient.ApplicationMessageReceivedAsync += (e) =>
        {
            var topic = e.ApplicationMessage.Topic;
            var message = Encoding.UTF8.GetString(e.ApplicationMessage.PayloadSegment);

            LastMessageOnTopic[topic] = message;

            PropertyChanged();

            return Task.CompletedTask;
        };

        BrokerClient.ConnectedAsync += (e) =>
        {
            

            return Task.CompletedTask;
        };
    }

    public async Task Connect()
    {
        var options = _factory.CreateClientOptionsBuilder()
            .WithClientId(Broker.ClientId)
            .WithWebSocketServer($"wss://{Broker.Server}:{Broker.Port}")
            .WithCredentials(Broker.Username, Broker.Password)
            .Build();

        await BrokerClient.ConnectAsync(options);
    }

    public async Task Disconnect()
    {
        await BrokerClient.DisconnectAsync();
    }


    private void PropertyChanged() => OnMessageReceived?.Invoke();
}
