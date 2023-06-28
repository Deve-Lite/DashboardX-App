using DashboardXModels.Brokers;
using DashboardXModels.Controls;
using DashboardXModels.Devices;
using MQTTnet;
using MQTTnet.Client;
using System.Text;

namespace DashboardX.Services;

public class MQTTService
{
    private readonly MqttFactory _factory;

    private IDictionary<string, IMqttClient> clients;
    public IDictionary<string, IMqttClient> Clients => clients;


    private IDictionary<string, string> lastMessages;
    public IDictionary<string, string> LastMessages => lastMessages;

    private HashSet<string> topics;
    public HashSet<string> Topics => topics;

    public MQTTService(MqttFactory factory)
    {
        _factory = factory;
        clients = new Dictionary<string, IMqttClient>();
        topics = new HashSet<string>();
        lastMessages = new Dictionary<string, string>();
    }

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

    public async Task UpdateClient(Broker broker)
    {
        await DeleteClient(broker.BrokerId);
        await CreateClient(broker);
    }

    public async Task DeleteClient(string id)
    {
        var client = clients[id];

        await client.DisconnectAsync();
        client.Dispose();

        clients.Remove(id);
    }

}
