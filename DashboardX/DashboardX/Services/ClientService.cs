using DashboardX.Brokers;
using DashboardX.Devices;
using DashboardXModels;
using DashboardXModels.Brokers;
using DashboardXModels.Controls;
using DashboardXModels.Devices;
using MQTTnet;
using MQTTnet.Client;
using System.Collections.ObjectModel;
using System.Text;

namespace DashboardX.Services;


//TODO thinkof if better is to return InitializedBroker where Needed Single Item 
public class ClientService
{
    private readonly MqttFactory _factory;
    private readonly IBrokerService _brokerService;
    private readonly IDeviceService _deviceService;

    private ICollection<InitializedBroker> clients;
    public ICollection<InitializedBroker> Clients => clients;

    public bool LoadedSuccessfully { get; private set; }

    public Action OnMessageReceived { get; set; }

    public ClientService(MqttFactory factory, IBrokerService brokerService, IDeviceService deviceService)
    {
        _deviceService = deviceService;
        _brokerService = brokerService;
        _factory = factory;
        clients = new Collection<InitializedBroker>();
    }

    public async Task Initialize()
    {
        var brokerResponse = await _brokerService.GetBrokers();
        var deviceResponse = await _deviceService.GetDevices(); 

        // TODO: In case of error show toast
    }

    public async Task<List<Broker>> GetBrokers()
    {
        var response = await _brokerService.GetBrokers();

        //TODO: In case of error show toast

        response.Data.ForEach(async x => await HandleBrokerConnection(x));

        return response.Data;
    }
    public async Task<Broker> GetBroker(string id)
    {
        var response = await _brokerService.GetBroker(id);
        var broker = response.Data;

        //TODO: In case of error show toast

        await HandleBrokerConnection(broker);

        return broker;
    }

    public async Task<IEnumerable<Device>> GetDevices(string brokerId)
    {
        //TODO: device methods should also call broker methods!! 

        var response = await _deviceService.GetDevices(brokerId);
        var devices = response.Data;

        foreach (var device in devices) 
        {

        }
        //TODO: In case of error show toast

        return response.Data;

    }

    public async Task<IEnumerable<Device>> GetDevice(string deviceId)
    {
        //TODO: device methods should also call broker methods!! 

        var response = await _deviceService.GetDevice(deviceId);
        var device = response.Data;

        //TODO: In case of error show toast


        return device;
    }

    public async Task<IMqttClient> Connect(Broker broker)
    {
        var client = _factory.CreateMqttClient();

        var options = _factory.CreateClientOptionsBuilder()
            .WithClientId(broker.ClientId)
            .WithWebSocketServer($"wss://{broker.Server}:{broker.Port}")
            .WithCredentials(broker.Username, broker.Password)
            .Build();

        client.ApplicationMessageReceivedAsync += (e) =>
        {
            var topic = e.ApplicationMessage.Topic;
            var message = Encoding.UTF8.GetString(e.ApplicationMessage.PayloadSegment);

            var hash = broker.BrokerId + topic;

            topics[hash] = message;

            return Task.CompletedTask;
        };

        //TODO : add reconnect logic
        client.DisconnectedAsync += async (e) =>
        {
            
        }

        await client.ConnectAsync(options);

        return client;
    }

    public async Task Disconnect(string brokerId)
    {
        var client = clients[brokerId];

        await client.DisconnectAsync();
    }

    public async Task RemoveBroker(string brokerId)
    {
        //TODO device and broker service should remove from here !
       await Disconnect(brokerId);
    }

    public async Task RemoveDevice()
    {
        //TODO device and broker service should remove from here !
        
    }


    [Obsolete]
    public async Task RefreshTopics(IEnumerable<Device> devices)
    {
        List<(string,string, QualityOfService)> brokersToTopic = new();

        foreach(var device in devices) 
            foreach(var control in device.GetControls())
            {
                var topic = string.IsNullOrEmpty(device.BaseDevicePath) ? control.Topic : $"{device.BaseDevicePath}/{control.Topic}";
                brokersToTopic.Add((device.BrokerId, topic, control.QualityOfService));
            }

        
        foreach(var (brokerId, topic, qualityOfService) in brokersToTopic)
        {
            if (!topics.Contains(topic))
            {
                topics.Add(topic);

                var client = clients[brokerId]; 

                var options = _factory.CreateTopicFilterBuilder()
                    .WithTopic(topic)
                    .WithQualityOfServiceLevel((MQTTnet.Protocol.MqttQualityOfServiceLevel) qualityOfService)
                    .Build();

                await client.SubscribeAsync(options);
                
            }
        }

        foreach(var topic in topics.ToList())
        {
            if (!brokersToTopic.Any(brokerToTopic => brokerToTopic.Item2 == topic))
            {
                //var client = clients[brokerId];
                
            }
        }

    }
    [Obsolete]
    public async Task RefreshClients(IEnumerable<Broker> brokers)
    {
        var brokerIds = new HashSet<string>(brokers.Select(broker => broker.BrokerId));
        var keysToRemove = clients.Keys.Where(key => !brokerIds.Contains(key)).ToList();

        foreach (var key in keysToRemove)
            await DeleteClient(key);
        
        foreach (var broker in brokers)
            if (!clients.ContainsKey(broker.BrokerId))
                await CreateClient(broker);
    }
    [Obsolete]
    public async Task CreateClient(Broker broker)
    {
        var client = _factory.CreateMqttClient();

        var options = _factory.CreateClientOptionsBuilder()
            .WithClientId(broker.ClientId)
            .WithTcpServer(broker.Server, broker.Port)
            .WithCredentials(broker.Username, broker.Password)
            .Build();

        client.ApplicationMessageReceivedAsync += (e) =>
        {
            var topic = e.ApplicationMessage.Topic;
            var message = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);


            LastMessages[topic] = message;

            return Task.CompletedTask;
        };

        await client.ConnectAsync(options);

        clients[broker.BrokerId] = client;
    }
    [Obsolete]
    public async Task UpdateClient(Broker broker)
    {
        await DeleteClient(broker.BrokerId);
        await CreateClient(broker);
    }
    [Obsolete]
    public async Task DeleteClient(string id)
    {
        var client = clients[id];

        await client.DisconnectAsync();
        client.Dispose();

        clients.Remove(id);
    }



    #region Privates 

    private async Task HandleBrokerConnection(Broker broker)
    {
        if (clients.ContainsKey(broker.BrokerId))
        {
            // TODO: Verify if changes occurred since the last connection
        }
        else
        {
            var client = await Connect(broker);
            clients[broker.BrokerId] = client;
        }
    }

    private async Task HandleDeviceConnection(Device device)
    {
        var controls = device.GetControls();

        foreach (var control in controls)
        {
            var topic = GetControlTopicPath(device, control);
            var hash = device.BrokerId + topic;

            if (topics.ContainsKey(hash))
            {
                // TODO: Verify if changes occurred since the last connection
            }
            else
            {
                var client = await Connect(broker);
                clients[broker.BrokerId] = client;
            }
        }
    }

    private string GetControlTopicPath(Device device, Control control) => string.IsNullOrEmpty(device.BaseDevicePath) ? control.Topic : $"{device.BaseDevicePath}/{control.Topic}";

    #endregion
}
