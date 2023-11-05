namespace Presentation.Clients;

//TODO: Thinkof extending class with localizer
//TODO: Split service 3 into smaller ones
public class ClientService : IClientService
{
    private readonly IBrokerService _brokerService;
    private readonly IDeviceService _deviceService;
    private readonly IClientFactory _clientFactory;
    private readonly ILogger<ClientService> _logger;
    private readonly List<IClient> _clients;

    public ClientService(IBrokerService brokerService,
                         IDeviceService deviceService,
                         ILogger<ClientService> logger,
                         IClientFactory clientFactory)
    {
        _brokerService = brokerService;
        _deviceService = deviceService;
        _clientFactory = clientFactory;
        _logger = logger;
        _clients = new List<IClient>();
    }

    public async Task Logout()
    {
        foreach (var client in _clients)
            await client.DisconnectAsync();

        _clients.Clear();
    }


    #region Client

    public async Task<Result<List<IClient>>> GetClientsWithDevices()
    {
        var brokersTask = _brokerService.GetBrokers();
        var devicesTask = _deviceService.GetDevices();

        await Task.WhenAll(brokersTask, devicesTask);

        var brokersResult = brokersTask.Result;
        var devicesResult = devicesTask.Result;

        if (!brokersResult.Succeeded)
            return Result<List<IClient>>.Fail(brokersResult.Messages, brokersResult.StatusCode);

        if (!devicesResult.Succeeded)
            return Result<List<IClient>>.Fail(devicesResult.Messages, devicesResult.StatusCode);

        var usedClients = new HashSet<string>();

        var devicesGroups = devicesResult.Data
            .Where(x => !string.IsNullOrEmpty(x.BrokerId))
            .GroupBy(x => x.BrokerId)
            .ToDictionary(group => group.Key, group => group.ToList());

        IResult finalStatus = Result.Success();

        foreach (var broker in brokersResult.Data)
        {
            var client = _clients.FirstOrDefault(x => x.Id == broker.Id);
            if (client is null)
            {
                var newClient = _clientFactory.GenerateClient(broker);
                _clients.Add(newClient);
                client = newClient;
            }
            else if (client.Broker.EditedAt != broker.EditedAt)
            {
                await client.UpdateBroker(broker);
            }

            if (devicesGroups.ContainsKey(broker.Id))
            {
                var tmpStatus = await UpdateAllControls(client, devicesGroups[broker.Id]);
                if (tmpStatus.OperationState != OperationState.Success)
                    finalStatus = tmpStatus;
            }

            usedClients.Add(broker.Id);
        }

        foreach (var client in _clients)
        {
            if (!usedClients.Contains(client.Id))
            {
                await client.DisposeAsync();
                _clients.Remove(client);
            }
        }

        if(finalStatus.Succeeded)
            return Result<List<IClient>>.Success(_clients, brokersResult.StatusCode);

        return Result<List<IClient>>.Fail(_clients, brokersResult.StatusCode);
    }

    public async Task<Result<List<IClient>>> GetClients()
    {
        var result = await _brokerService.GetBrokers();

        if (!result.Succeeded)
            return Result<List<IClient>>.Fail(result.Messages, result.StatusCode);

        var brokers = result.Data;

        var usedClients = new HashSet<string>();

        foreach (var broker in brokers)
        {
            var client = _clients.FirstOrDefault(x => x.Id == broker.Id);
            if (client is null)
                _clients.Add(_clientFactory.GenerateClient(broker));
            else
                await client.UpdateBroker(broker);

            usedClients.Add(broker.Id);
        }

        foreach (var client in _clients.ToList())
        {
            if (!usedClients.Contains(client.Id))
            {
                await client.DisposeAsync();
                _clients.Remove(client);
            }
        }

        return Result<List<IClient>>.Success(_clients, result.StatusCode);
    }

    public async Task<Result<IClient>> GetClient(string brokerId)
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

        var client = _clients.FirstOrDefault(x => x.Id == brokerId);

        if (client is null)
        {
            client = _clientFactory.GenerateClient(brokerResult.Data);
            _clients.Add(client);
        }
        
        await client.UpdateBroker(brokerResult.Data);

        var result = await UpdateAllControls(client, deviceResult.Data);

        if (result.OperationState == OperationState.Success)
            return Result<IClient>.Success(client);

        if (result.OperationState == OperationState.Warning)
            return Result<IClient>.Warning(client);

        return Result<IClient>.Fail(client);
    }

    public async Task<IResult> UpdateClient(BrokerDTO broker, BrokerCredentialsDTO brokerCredentialsDTO)
    {
        var result = await _brokerService.UpdateBroker(broker);

        if (!result.Succeeded)
            return Result.Fail(result.Messages, result.StatusCode);

        var credResult = await _brokerService.UpdateBrokerCredentials(result.Data.Id, brokerCredentialsDTO);

        await _clients.First(x => x.Id == broker.Id)!.UpdateBroker(result.Data);

        if (!credResult.Succeeded)
            return Result.Warning(message: "Failed to update broker credentilas");

        return Result.Success(result.StatusCode);
    }

    public async Task<IResult> CreateClient(BrokerDTO broker, BrokerCredentialsDTO brokerCredentialsDTO)
    {
        var result = await _brokerService.CreateBroker(broker);

        if (!result.Succeeded)
            return Result.Fail(result.Messages, result.StatusCode);

        var credResult = await _brokerService.UpdateBrokerCredentials(result.Data.Id, brokerCredentialsDTO);

        var client = _clientFactory.GenerateClient(result.Data);
        _clients.Add(client);

        if (!credResult.Succeeded)
            return Result.Warning(message: "Failed to create broker credentilas but creaded broker.");

        return Result.Success();
    }

    public async Task<Result> RemoveClient(string brokerId)
    {
        var result = await _brokerService.RemoveBroker(brokerId);

        if (!result.Succeeded)
            return Result.Fail(result.Messages, result.StatusCode);

        var client = _clients.First(x => x.Broker.Id == brokerId);
        await client.DisposeAsync();
        _clients.Remove(client);

        return Result.Success();
    }

    #endregion

    #region Device

    public async Task<IResult> RemoveDevice(string clientId, string deviceId)
    {
        var result = await _deviceService.RemoveDevice(deviceId);

        if (!result.Succeeded)
            return result;

        var removeResult = await _clients.First(x => x.Id == clientId)
            .RemoveDevice(deviceId);

        if (removeResult.OperationState != OperationState.Success)
            return removeResult;

        return Result.Success(result.StatusCode);
    }

    public async Task<IResult> CreateDevice(DeviceDTO device)
    {
        var result = await _deviceService.CreateDevice(device);

        if (!result.Succeeded)
            Result<Device>.Fail(result.Messages, result.StatusCode);

        var addResult = _clients.First(x => x.Id == device.BrokerId)
            .AddDevice(result.Data);

        return addResult;

    }

    public async Task<IResult> UpdateDevice(DeviceDTO device)
    {
        var result = await _deviceService.UpdateDevice(device);

        if (!result.Succeeded)
            return Result.Fail(result.Messages, result.StatusCode);

        var updateResult = await _clients.First(x => x.Id == device.BrokerId)
            .UpdateDevice(result.Data);

        if (updateResult.OperationState == OperationState.Warning)
        {
            //TODO: Localizer
            Result<Device>.Warning(message: $"Failed to subsribe topics.");
        }

        return updateResult;

    }

    #endregion

    #region Control

    public async Task<IResult> RemoveControl(string clientId, Control control)
    {
        var result = await _deviceService.RemoveDeviceControl(control.DeviceId, control.Id);

        if (!result.Succeeded)
            return Result.Fail(result.Messages, result.StatusCode);

        return await _clients.First(x => x.Id == clientId)
            .RemoveControl(control.Id);
    }

    public async Task<IResult> CreateControlForDevice(string clientId, Control control)
    {
        var result = await _deviceService.CreateDeviceControl(control);

        if (!result.Succeeded)
            return Result.Fail(result.Messages, result.StatusCode);

        return await _clients.First(x => x.Id == clientId)
            .AddControl(control);
    }

    public async Task<IResult> UpdateControlForDevice(string clientId, Control control)
    {
        var result = await _deviceService.UpdateDeviceControl(control);

        if (result.Succeeded)
            return Result.Fail(result.Messages, result.StatusCode);

        return await _clients.First(x => x.Id == clientId)
            .UpdateControl(control);
    }

    #endregion

    #region Privates

    private async Task<IResult> UpdateAllControls(IClient client, List<Device> devices)
    {
        HashSet<string> usedDevices = new();

        var finalStatus = Result.Success();

        foreach (var device in devices)
        {
            var result = await _deviceService.GetDeviceControls(device.Id);

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

        foreach (var device in client.Devices)
            if (!usedDevices.Contains(device.Id))
                await client.RemoveDevice(device.Id);

        return finalStatus;
    }

    #endregion
}
