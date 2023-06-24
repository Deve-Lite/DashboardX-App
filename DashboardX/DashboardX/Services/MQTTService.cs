using MQTTnet;
using MQTTnet.Client;

namespace DashboardX.Services;

public class MQTTService
{
    private readonly MqttFactory _factory;

    private IDictionary<string, IMqttClient> _clients;
    public IDictionary<string, IMqttClient> Clients => _clients;

    public MQTTService(MqttFactory factory)
    {
        _factory = factory;
        _clients = new Dictionary<string, IMqttClient>();
    }

    public async Task CreateClient(string clientId, MqttClientOptionsBuilder options)
    {
        var client = _factory.CreateMqttClient();

        await client.ConnectAsync(options.Build());

        _clients[clientId] = client;
    }
}
