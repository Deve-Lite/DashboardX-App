using Core.Interfaces;
using Infrastructure;
using MQTTnet;
using MQTTnet.Client;
using Presentation.Models;
using Presentation.Services.Interfaces;
using Shared.Models.Brokers;
using System.Text;

namespace Presentation.Services;

public class ClientService : IClientService
{
    private readonly IBrokerService _brokerService;
    private readonly ITopicService _topicService;
    private readonly IDeviceService _deviceService;
    private readonly MqttFactory _factory;

    private List<BrokerClient> _brokers = new();
    private List<DeviceClient> _devices = new();

    public ClientService(ITopicService topicService, IBrokerService brokerService, IDeviceService deviceService, MqttFactory factory)
    {
        _brokerService = brokerService;
        _topicService = topicService;
        _deviceService = deviceService;
        _factory = factory;
    }

    public async Task<Result<List<BrokerClient>>> GetBrokers()
    {
        var result = await _brokerService.GetBrokers();

        if(!result.Succeeded)
            return Result<List<BrokerClient>>.Fail(result.StatusCode, result.Messages);

        //TODO: UPDATE Brokers list 
        _brokers = result.Data.Select(x => new BrokerClient { Broker = x}).ToList();

        return Result<List<BrokerClient>>.Success(result.StatusCode, _brokers);
    }

    public async Task<Result<BrokerClient>> GetBroker(string brokerId)
    {
        var result = await _brokerService.GetBroker(brokerId);

        if (!result.Succeeded)
            return Result<BrokerClient>.Fail(result.StatusCode, result.Messages);

        //TODO: UPDATE Brokers list 
        var data = new BrokerClient { Broker = result.Data };

        return Result<BrokerClient>.Success(result.StatusCode, data);
    }

    public async Task<Result> RemoveBroker(string brokerId)
    {
        var result = await _brokerService.RemoveBroker(brokerId);

        if(result.Succeeded)
        {
            //TODO: disconnect
            var index = _brokers.FindIndex(x => x.Broker.Id == brokerId);

            if(index != -1)
                _brokers.RemoveAt(index);

            return Result.Success(result.StatusCode);
        }

        return Result.Fail(result.StatusCode, result.Messages);
    }

    public async Task<Result<BrokerClient>> CreateBroker(Broker broker)
    {
        var result = await _brokerService.CreateBroker(broker);

        if(result.Succeeded)
        {
            var data = new BrokerClient 
            {
                Broker = result.Data 
            };

            return Result<BrokerClient>.Success(result.StatusCode, data);
        }

        return Result<BrokerClient>.Fail(result.StatusCode, result.Messages);
    }

    public async Task<Result<BrokerClient>> UpdateBroker(Broker broker)
    {
        var result = await _brokerService.CreateBroker(broker);

        if (result.Succeeded)
        {
            var data = new BrokerClient
            {
                Broker = result.Data
            };

            return Result<BrokerClient>.Success(result.StatusCode, data);
        }

        return Result<BrokerClient>.Fail(result.StatusCode, result.Messages);
    }

    #region Privates

    private async Task CreateBrokerClient(Broker broker)
    {
        var service = _factory.CreateMqttClient();

        var options = _factory.CreateClientOptionsBuilder()
            .WithClientId(broker.ClientId)
            .WithWebSocketServer($"wss://{broker.Server}:{broker.Port}")
            .WithCredentials(broker.Username, broker.Password)
            .Build();

        await service.ConnectAsync(options);

        InitializeCallbacks(service, broker);

        var client = new BrokerClient
        {
            Broker = broker,
            MqttService = service
        };

        _brokers.Add(client);
    }
    private async Task<IMqttClient> ConnectMqttClient(Broker broker)
    {
        var client = _factory.CreateMqttClient();

        var options = _factory.CreateClientOptionsBuilder()
            .WithClientId(broker.ClientId)
            .WithWebSocketServer($"wss://{broker.Server}:{broker.Port}")
            .WithCredentials(broker.Username, broker.Password)
            .Build();

        await client.ConnectAsync(options);

        InitializeCallbacks(client, broker);

        return client;
    }
    private void InitializeCallbacks(IMqttClient client, Broker broker)
    {
        client.ApplicationMessageReceivedAsync += (e) =>
        {
            var topic = e.ApplicationMessage.Topic;

            var message = Encoding.UTF8.GetString(e.ApplicationMessage.PayloadSegment);

            _topicService.UpdateMessageOnTopic(broker.Id, topic, message);

            //TODO: on message received
            OnMessageReceived.Invoke();

            return Task.CompletedTask;
        };
        client.DisconnectedAsync += Disconnect;
    }

    private Task Disconnect(MqttClientDisconnectedEventArgs e)
    {
        return Task.CompletedTask;
    }

    #endregion
}
