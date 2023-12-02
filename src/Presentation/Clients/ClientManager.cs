namespace Presentation.Clients;

public class ClientManager : IClientManager
{
    private readonly MqttFactory _factory;
    private readonly IFetchBrokerService _brokerService;
    private readonly ILocalStorageService _localStorage;
    private readonly IStringLocalizer<Client> _localizer;
    private readonly ILogger<Client> _clientLogger;
    private readonly IList<IClient> _clients;

    public ClientManager(IFetchBrokerService brokerService,
                         ILocalStorageService storage,
                         IStringLocalizer<Client> localizer,
                         ILogger<Client> clientLogger,
                         MqttFactory factory)
    {
        _factory = factory;
        _clientLogger = clientLogger;
        _brokerService = brokerService;
        _localStorage = storage;
        _localizer = localizer;
        _clients = new List<IClient>();
    }

    public IResult<IClient> AddClient(Broker broker)
    {
        var topicService = new TopicService(_localStorage);
        var mqttClient = _factory.CreateMqttClient();
        var client = new Client(topicService, mqttClient, _brokerService, _clientLogger, _localizer, broker);

        _clients.Add(client);

        return Result<IClient>.Success(client);
    }
    public async Task<IResult<IClient>> UpdateClient(Broker broker)
    {
        if(!_clients.Any(x => x.Id == broker.Id))
            return AddClient(broker);

        var client = _clients.First(x => x.Id == broker.Id);
        
        await client.UpdateBroker(broker);

        return Result<IClient>.Success(client);
    }
    public IResult<IClient> GetClient(string clientId)
    {
        if(_clients.Any(x => x.Id == clientId))
            return Result<IClient>.Success(_clients.First(x => x.Id == clientId));

        return Result<IClient>.Fail();
    }
    public IResult<IList<IClient>> GetClients() => Result<IList<IClient>>.Success(_clients);
    public async Task<IResult> RemoveClient(string clientId)
    {
        if (!_clients.Any(x => x.Id == clientId))
            return Result.Success();

        var client = _clients.First(x => x.Id == clientId);
        _clients.Remove(client);

        await client.DisposeAsync();

        return Result.Success();
    }

    public IResult<IList<IClient>> RemoveClients()
    {
        var clients = _clients.ToList();
        _clients.Clear();
        return Result<IList<IClient>>.Success(clients);
    }
}
