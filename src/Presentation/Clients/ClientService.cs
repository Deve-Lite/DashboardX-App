namespace Presentation.Clients;

//TODO: Thinkof extending class with localizer
public class ClientService : IClientService
{
    private readonly IFetchBrokerService _brokerService;
    private readonly IFetchDeviceService _deviceService;
    private readonly IFetchControlService _controlService;
    private readonly IClientManager _clientManager;
    private readonly ILogger<ClientService> _logger;

    public ClientService(IFetchBrokerService brokerService,
                         IFetchDeviceService deviceService,
                         IFetchControlService controlService,
                         IClientManager clientManager,
                         ILogger<ClientService> logger)
    {
        _brokerService = brokerService;
        _deviceService = deviceService;
        _clientManager = clientManager;
        _controlService = controlService;
        _logger = logger;
    }

    public async Task Logout()
    {
        var clientsResult = _clientManager.GetClients();

        if(!clientsResult.Succeeded)
            return;

        foreach (var client in clientsResult.Data)
            await client.DisconnectAsync();

        clientsResult.Data.Clear();
    }
    public async Task<IResult<IList<IClient>>> GetClientsWithDevices()
    {
        var brokersTask = _brokerService.GetBrokers();
        var devicesTask = _deviceService.GetDevices();

        await Task.WhenAll(brokersTask, devicesTask);

        var brokersResult = brokersTask.Result;
        var devicesResult = devicesTask.Result;

        if (!brokersResult.Succeeded)
            return Result<IList<IClient>>.Fail(brokersResult.Messages, brokersResult.StatusCode);

        if (!devicesResult.Succeeded)
            return Result<IList<IClient>>.Fail(devicesResult.Messages, devicesResult.StatusCode);

        var usedClients = new HashSet<string>();

        var devicesGroups = devicesResult.Data
            .Where(x => !string.IsNullOrEmpty(x.BrokerId))
            .GroupBy(x => x.BrokerId)
            .ToDictionary(group => group.Key, group => group.ToList());

        IResult finalStatus = Result.Success();

        foreach (var broker in brokersResult.Data)
        {
            var clientResult = _clientManager.GetClient(broker.Id);

            IResult<IClient> resultClient = Result<IClient>.Success();

            if (!clientResult.Succeeded)
                resultClient = _clientManager.AddClient(broker);
            else
                resultClient = await _clientManager.UpdateClient(broker);

            usedClients.Add(broker.Id);

            if (devicesGroups.ContainsKey(broker.Id))
            {
                var tmpStatus = await UpdateAllControls(resultClient.Data, devicesGroups[broker.Id]);
                if (tmpStatus.OperationState != OperationState.Success)
                    finalStatus = tmpStatus;
            }

            usedClients.Add(broker.Id);
        }

        await RemoveUnusedClients(usedClients);

        var clientsResult = _clientManager.GetClients();

        if (finalStatus.Succeeded)
            return Result<IList<IClient>>.Success(clientsResult.Data, brokersResult.StatusCode);

        return Result<IList<IClient>>.Fail(clientsResult.Data, brokersResult.StatusCode);
    }
    public async Task<IResult<IList<IClient>>> GetClients()
    {
        var result = await _brokerService.GetBrokers();

        if (!result.Succeeded)
            return Result<IList<IClient>>.Fail(result.Messages, result.StatusCode);

        var brokers = result.Data;

        var usedClients = new HashSet<string>();

        foreach (var broker in brokers)
        {
            var clientResult = _clientManager.GetClient(broker.Id);

            if (!clientResult.Succeeded)
                _clientManager.AddClient(broker);
            else
              await _clientManager.UpdateClient(broker);
            
            usedClients.Add(broker.Id);
        }

        await RemoveUnusedClients(usedClients);

        var clientsResult = _clientManager.GetClients();

        return Result<IList<IClient>>.Success(clientsResult.Data, result.StatusCode);
    }
    public async Task<IResult<IClient>> GetClient(string brokerId)
    {
        var brokerTask = _brokerService.GetBroker(brokerId);
        var deviceTask = _deviceService.GetDevices(brokerId);

        await Task.WhenAll(brokerTask, deviceTask);

        var brokerResult = brokerTask.Result;
        var deviceResult = deviceTask.Result;

        if (!brokerResult.Succeeded)
            return Result<IClient>.Fail(brokerResult.Messages, brokerResult.StatusCode);

        if (!deviceResult.Succeeded)
            return Result<IClient>.Fail(deviceResult.Messages, deviceResult.StatusCode);

        var clientResult = _clientManager.GetClient(brokerId);

        if (!clientResult.Succeeded)
        {
            var addResult = _clientManager.AddClient(brokerResult.Data);

            if(!addResult.Succeeded)
                return Result<IClient>.Fail(addResult.Messages, addResult.StatusCode);

            var client = addResult.Data;

            var result = await UpdateAllControls(client, deviceResult.Data);

            if (!result.Succeeded)
                Result<IClient>.Fail(client);
            
            return Result<IClient>.Success(client);
        }
        else
        {
            var client = clientResult.Data;
            await client.UpdateBroker(brokerResult.Data);
            var result = await UpdateAllControls(client, deviceResult.Data);

            if (!result.Succeeded)
                Result<IClient>.Fail(client);

            return Result<IClient>.Success(client);
        }
    }

    private async Task RemoveUnusedClients(HashSet<string> usedClients)
    {
        var clientsResult = _clientManager.GetClients();

        var ids = clientsResult.Data.Select(x => x.Id).ToList();

        foreach (var clientId in ids)
        {
            if (!usedClients.Contains(clientId))
                await _clientManager.RemoveClient(clientId);
        }
    }
    private async Task<IResult> UpdateAllControls(IClient client, List<Device> devices)
    {
        HashSet<string> usedDevices = new();

        var finalStatus = Result.Success();

        foreach (var device in devices)
        {
            var result = await _controlService.GetControls(device.Id);

            if(!result.Succeeded)
            {
                //TODO: Handle result;
                continue;
            }    

            var hasDevice = client.HasDevice(device.Id);

            if (hasDevice)
            {
                var updateResult = await client.UpdateDevice(device, result.Data);
                //TODO: Handle updateResult
            }
            else
            {
                var addResult = await client.AddDevices(device, result.Data);
                //TODO: Handle addResult
            }

            usedDevices.Add(device.Id);
        }

        foreach (var device in client.GetDevices())
            if (!usedDevices.Contains(device.Id))
                await client.RemoveDevice(device.Id);

        return finalStatus;
    }
}
