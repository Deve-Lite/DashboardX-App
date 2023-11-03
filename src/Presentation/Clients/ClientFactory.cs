namespace Presentation.Clients;

public class ClientFactory : IClientFactory
{
    private readonly MqttFactory _factory;
    private readonly ILogger<Client> _clientLogger;
    private readonly IBrokerService _brokerService;
    private readonly ILocalStorageService _localStorage;

    public ClientFactory(IBrokerService brokerService,
                         ILocalStorageService storage,
                         ILogger<Client> clientLogger,
                         MqttFactory factory)
    {
        _factory = factory;
        _clientLogger = clientLogger;
        _brokerService = brokerService;
        _localStorage = storage;
    }

    public IClient GenerateClient(Broker broker)
    {
        var topicService = new TopicService(_localStorage);
        var mqttClient = _factory.CreateMqttClient();
        return new Client(topicService, mqttClient, _brokerService, _clientLogger, broker);
    }
}
