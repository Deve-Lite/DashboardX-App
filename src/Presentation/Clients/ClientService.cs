﻿namespace Presentation.Clients;

public class ClientService : IClientService
{
    private readonly IBrokerService _brokerService;
    private readonly ILocalStorageService _storageService;
    private readonly IDeviceService _deviceService;
    private readonly ILogger<ClientService> _logger;
    private readonly ILogger<Client> _clientLogger;
    private readonly MqttFactory _factory;
    private readonly List<Client> _clients = new();

    public ClientService(IBrokerService brokerService,
        ILocalStorageService storageService,
        IDeviceService deviceService,
        ILogger<ClientService> logger,
        ILogger<Client> clientLogger,
        MqttFactory factory)
    {
        _brokerService = brokerService;
        _storageService = storageService;
        _deviceService = deviceService;
        _factory = factory;
        _logger = logger;
        _clientLogger = clientLogger;
    }

    public async Task Logout()
    {
        foreach (var client in _clients)
            await client.DisconnectAsync();

        _clients.Clear();
    }


    #region Client

    public async Task<Result<List<Client>>> GetClientsWithDevices()
    {
        var brokersTask = _brokerService.GetBrokers();
        var devicesTask = _deviceService.GetDevices();

        await Task.WhenAll(brokersTask, devicesTask);

        var brokersResult = brokersTask.Result;
        var devicesResult = devicesTask.Result;

        if (!brokersResult.Succeeded)
            return Result<List<Client>>.Fail(brokersResult.Messages, brokersResult.StatusCode);

        if (!devicesResult.Succeeded)
            return Result<List<Client>>.Fail(devicesResult.Messages, devicesResult.StatusCode);

        var usedClients = new HashSet<string>();

        var devicesGroups = devicesResult.Data
            .GroupBy(x => x.BrokerId)
            .ToDictionary(group => group.Key, group => group.ToList());

        var failedConnections = 0;

        foreach (var broker in brokersResult.Data)
        {
            var client = _clients.FirstOrDefault(x => x.Id == broker.Id);
            if (client is null)
            {
                var newClient = new Client(_storageService, _clientLogger, _factory, broker);
                _clients.Add(newClient);
                client = newClient;
            }
            else if (client.Broker.EditedAt != broker.EditedAt)
            {
                await client.UpdateBroker(broker);
            }

            if (devicesGroups.ContainsKey(broker.Id))
                failedConnections += await UpdateClientDevices(client, devicesGroups[broker.Id]);

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

        if (failedConnections == 0)
            return Result<List<Client>>.Success(_clients, brokersResult.StatusCode);

        return Result<List<Client>>.Warning(_clients, message: $"Failed to subscribe {failedConnections} topics.");
    }

    public async Task<Result<List<Client>>> GetClients()
    {
        var result = await _brokerService.GetBrokers();

        if (!result.Succeeded)
            return Result<List<Client>>.Fail(result.Messages, result.StatusCode);

        var brokers = result.Data;

        var usedClients = new HashSet<string>();

        foreach (var broker in brokers)
        {
            var client = _clients.FirstOrDefault(x => x.Id == broker.Id);
            if (client is null)
            {
                var newClient = new Client(_storageService, _clientLogger, _factory, broker);
                _clients.Add(newClient);
            }
            else if (client.Broker.EditedAt != broker.EditedAt)
            {
                await client.UpdateBroker(broker);
            }

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

        return Result<List<Client>>.Success(_clients, result.StatusCode);
    }

    public async Task<Result<Client>> GetClient(string brokerId)
    {
        var brokerTask = _brokerService.GetBroker(brokerId);
        var deviceTask = _deviceService.GetDevices(brokerId);

        await Task.WhenAll(brokerTask, deviceTask);

        var brokerResult = brokerTask.Result;
        var deviceResult = deviceTask.Result;

        if (!brokerResult.Succeeded)
            return Result<Client>.Fail(brokerResult.Messages, brokerResult.StatusCode);

        if (!deviceResult.Succeeded)
            return Result<Client>.Fail(deviceResult.Messages, deviceResult.StatusCode);

        var client = _clients.FirstOrDefault(x => x.Id == brokerId);

        if (client is null)
        {
            var newClient = new Client(_storageService, _clientLogger, _factory, brokerResult.Data);

            var failedConnections = await UpdateClientDevices(newClient, deviceResult.Data);

            _clients.Add(newClient);

            if (failedConnections == 0)
                return Result<Client>.Success(newClient);

            return Result<Client>.Warning(newClient, message: $"Failed to subsribe {failedConnections} topics.");
        }
        else if (client.Broker.EditedAt != brokerResult.Data.EditedAt)
            await client.UpdateBroker(brokerResult.Data);


        var failConnections = await UpdateClientDevices(client, deviceResult.Data);

        if (failConnections == 0)
            return Result<Client>.Success(client);

        return Result<Client>.Warning(client, message: $"Failed to subsribe {failConnections} topics.");
    }

    public async Task<Result<Client>> UpdateClient(Broker broker)
    {
        var result = await _brokerService.UpdateBroker(broker);

        if (result.Succeeded)
        {
            var client = _clients.FirstOrDefault(x => x.Id == broker.Id)!;

            await client.UpdateBroker(result.Data);

            return Result<Client>.Success(client, result.StatusCode);
        }

        return Result<Client>.Fail(result.Messages, result.StatusCode);
    }

    public async Task<Result> RemoveClient(string brokerId)
    {
        var result = await _brokerService.RemoveBroker(brokerId);

        if (result.Succeeded)
        {
            var client = _clients.First(x => x.Broker.Id == brokerId);
            await client.DisposeAsync();
            _clients.Remove(client);

            return Result.Success(result.StatusCode);
        }

        return Result.Fail(result.Messages, result.StatusCode);
    }

    public async Task<Result<Client>> CreateClient(Broker broker)
    {
        var result = await _brokerService.CreateBroker(broker);

        if (result.Succeeded)
        {
            var client = new Client(_storageService, _clientLogger, _factory, result.Data);
            _clients.Add(client);

            return Result<Client>.Success(client, result.StatusCode);
        }

        return Result<Client>.Fail(result.Messages, result.StatusCode);
    }

    #endregion

    #region Device

    public async Task<Result> RemoveDeviceFromClient(string clientId, Device device)
    {
        var result = await _deviceService.RemoveDevice(device.Id);

        if (result.Succeeded)
        {
            var client = _clients.First(x => x.Id == clientId);
            await client.UnsubscribeAsync(device);
            client.Devices.RemoveAll(x => x.Id == device.Id);

            return Result.Success(result.StatusCode);
        }

        return Result.Fail(result.Messages, result.StatusCode);
    }

    public async Task<Result<Device>> CreateDeviceForClient(Device device)
    {
        var result = await _deviceService.CreateDevice(device);

        if (result.Succeeded)
        {
            var client = _clients.First(x => x.Id == device.BrokerId);
            client.Devices.Add(result.Data);

            return (Result<Device>)result;
        }

        return Result<Device>.Fail(result.Messages, result.StatusCode);
    }

    public async Task<Result<Device>> UpdateDeviceForClient(Device device)
    {
        var result = await _deviceService.UpdateDevice(device);

        if (result.Succeeded)
        {
            var client = _clients.First(x => x.Id == device.BrokerId);

            var unsuccessfullConnections = await UpdateClientDevices(client, new List<Device> { result.Data });

            if (unsuccessfullConnections == 0)
                return (Result<Device>)Result.Success(result.StatusCode);

            return Result<Device>.Warning(message: $"Failed to subsribe {unsuccessfullConnections} topics.");
        }

        return Result<Device>.Fail(result.Messages, result.StatusCode);
    }

    #endregion

    #region Control

    public async Task<Result> RemoveControlFromDevice(string clientId, string deviceId, Control control)
    {
        var result = await _deviceService.RemoveDeviceControls(deviceId, new List<string> { clientId });

        if (result.Succeeded)
        {
            var client = _clients.First(x => x.Id == clientId);
            await client.UnsubscribeAsync(deviceId, control.Id);

            return Result.Success(result.StatusCode);
        }

        return Result.Fail(result.Messages, result.StatusCode);
    }

    public async Task<Result> CreateControlForDevice(string clientId, string deviceId, Control control)
    {
        var result = await _deviceService.CreateDeviceControl(control);

        if (result.Succeeded)
        {
            var client = _clients.First(x => x.Id == clientId);
            var device = client.Devices.First(x => x.Id == deviceId);


            if (!await client.SubscribeAsync(device, control))
                return Result.Fail(new List<string> { "Failed to subscribe message however, control was created." }, result.StatusCode);

            return Result.Success(result.StatusCode);
        }

        return Result.Fail(result.Messages, result.StatusCode);
    }

    public async Task<Result> UpdateControlForDevice(string clientId, string deviceId, Control control)
    {
        var result = await _deviceService.UpdateDeviceControl(control);

        if (result.Succeeded)
        {
            var client = _clients.First(x => x.Id == clientId);
            var device = client.Devices.First(x => x.Id == deviceId);
            await client.UnsubscribeAsync(deviceId, control.Id);

            return Result.Success(result.StatusCode);
        }

        return Result.Fail(result.Messages, result.StatusCode);
    }

    #endregion

    #region Privates

    private async Task<int> UpdateClientDevices(Client client, List<Device> devices)
    {
        HashSet<string> usedDevices = new();

        int failedSubscribtions = 0;

        foreach (var device in devices)
        {
            var existingDevice = client.Devices.FirstOrDefault(x => x.Id == device.Id);

            _logger.LogDebug($"Updating {device.Id} {device.Name}");

            if (existingDevice is null)
            {
                var result = await _deviceService.GetDeviceControls(device.Id);

                if (result.Succeeded)
                {
                    failedSubscribtions += await client.SubscribeAsync(device, result.Data);

                    if (failedSubscribtions != 0)
                        _logger.LogWarning($"Failed to subscribe {failedSubscribtions} topics. For {device.Id} {device.Name}.");

                    device.SuccessfullControlsFetch = true;
                }
                else
                {
                    device.SuccessfullControlsFetch = false;
                }

            }
            else if (device.EditedAt != existingDevice.EditedAt)
            {
                await client.UnsubscribeAsync(existingDevice);

                var result = await _deviceService.GetDeviceControls(device.Id);

                if (result.Succeeded)
                {
                    failedSubscribtions += await client.SubscribeAsync(device, result.Data);

                    if (failedSubscribtions != 0)
                        _logger.LogWarning($"Failed to subscribe {failedSubscribtions} topics. For {device.Id} {device.Name}.");

                    device.SuccessfullControlsFetch = true;
                }
                else
                {
                    device.SuccessfullControlsFetch = false;
                }
            }
            else
            {
                var result = await _deviceService.GetDeviceControls(device.Id);

                if (result.Succeeded)
                {
                    failedSubscribtions += await client.UpdateSubscribtionsAsync(device.Id, result.Data);

                    if (failedSubscribtions != 0)
                        _logger.LogWarning($"Failed to subscribe {failedSubscribtions} topics. For {device.Id} {device.Name}.");

                    device.SuccessfullControlsFetch = true;
                }
                else
                {
                    device.SuccessfullControlsFetch = false;
                }
            }

            usedDevices.Add(device.Id);
        }

        foreach (var device in client.Devices)
            if (!usedDevices.Contains(device.Id))
                client.Devices.Remove(device);

        return failedSubscribtions;
    }



    #endregion
}
