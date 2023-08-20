using Core.Interfaces;
using Microsoft.Extensions.Options;
using MQTTnet;
using MQTTnet.Client;
using Shared.Models.Brokers;
using Shared.Models.Devices;
using System.Text;

namespace Presentation.Models;

public class Client : IAsyncDisposable
{

    private readonly MqttFactory _factory;

    public string Id => Broker.Id;
    public bool IsConnected => Service.IsConnected;

    public Broker Broker { get; set; } = new();
    public IMqttClient Service { get; set; }
    public List<Device> Devices { get; set; } = new();

    public Client()
    {
        Devices = new List<Device>();
        Broker = new Broker();
        Service = new MqttFactory().CreateMqttClient();
    }

    public Client(Broker broker, ITopicService topicService, MqttFactory factory)
    {
        Broker = broker;
        Service = factory.CreateMqttClient();

        _factory = factory;

        InitializeCallbacks(topicService);
    }

    public async Task<MqttClientConnectResult> ConnectAsync() => await Service.ConnectAsync(Options());
    public async Task DisconnectAsync() => await Service.DisconnectAsync();

    public async ValueTask DisposeAsync()
    {
        //TODO: Remove topics ??;

        await Service.DisconnectAsync();
        Service.Dispose();
    }

    #region Privates

    private MqttClientOptions Options()
    {
        var optionsBuilder = _factory.CreateClientOptionsBuilder()
            .WithClientId(Broker.ClientId)
            .WithWebSocketServer($"wss://{Broker.Server}:{Broker.Port}");

        if (!string.IsNullOrEmpty(Broker.Username) && !string.IsNullOrEmpty(Broker.Password))
            optionsBuilder = optionsBuilder.WithCredentials(Broker.Username, Broker.Password);

        return optionsBuilder.Build();
    }

    private void InitializeCallbacks(ITopicService topicService)
    {
        Service.ApplicationMessageReceivedAsync += (e) =>
        {
            var topic = e.ApplicationMessage.Topic;
            var message = Encoding.UTF8.GetString(e.ApplicationMessage.PayloadSegment);
            topicService.UpdateMessageOnTopic(Broker.Id, topic, message);

            return Task.CompletedTask;
        };
    }



    #endregion
}
