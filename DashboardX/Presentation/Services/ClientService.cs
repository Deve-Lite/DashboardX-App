using Core;
using Core.Interfaces;
using Infrastructure;
using MQTTnet;
using Presentation.Models;
using Presentation.Services.Interfaces;
using Shared.Models.Brokers;
using Shared.Models.Devices;
using System.Net;

namespace Presentation.Services;

//TODO: Update services for not modified responses
public class ClientService : IClientService
{
    private readonly IBrokerService _brokerService;
    private readonly ITopicService _topicService;
    private readonly IDeviceService _deviceService;
    private readonly MqttFactory _factory;

    private List<Client> _clients = new();

    public ClientService(ITopicService topicService, IBrokerService brokerService, IDeviceService deviceService, MqttFactory factory)
    {
        _brokerService = brokerService;
        _topicService = topicService;
        _deviceService = deviceService;
        _factory = factory;
    }

    public async Task<Result<List<Client>>> GetClientsWithDevices()
    {
        var brokersTask = _brokerService.GetBrokers();
        var devicesTask = _deviceService.GetDevices();

        await Task.WhenAll(brokersTask, devicesTask);

        var brokersResult = brokersTask.Result;
        var devicesResult = devicesTask.Result;

        if (!brokersResult.Succeeded)
            return Result<List<Client>>.Fail(brokersResult.StatusCode, brokersResult.Messages);

        if (!devicesResult.Succeeded)
            return Result<List<Client>>.Fail(devicesResult.StatusCode, devicesResult.Messages);

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
                var newClient = new Client(broker, _topicService, _factory);
                _clients.Add(newClient);
                client = newClient;
            }
            else if (client.Broker.EditedAt != broker.EditedAt)
            {
                await UpdateClientBroker(client, broker);
            }

            if(devicesGroups.ContainsKey(broker.Id))
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
            return Result<List<Client>>.Success(brokersResult.StatusCode, _clients);

        return Result<List<Client>>.Fail($"Failed to subsribe {failedConnections} topics.");
    }

    public async Task<Result<List<Client>>> GetClients()
    {
        var result = await _brokerService.GetBrokers();

        if (!result.Succeeded)
            return Result<List<Client>>.Fail(result.StatusCode, result.Messages);

        var brokers = result.Data;

        var usedClients = new HashSet<string>();

        foreach (var broker in brokers)
        {
            var client = _clients.FirstOrDefault(x => x.Id == broker.Id);
            if(client is null)
            {
                var newClient = new Client(broker, _topicService, _factory);
                _clients.Add(newClient);
            }
            else if(client.Broker.EditedAt != broker.EditedAt)
            {
                await UpdateClientBroker(client, broker);
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

        return Result<List<Client>>.Success(result.StatusCode, _clients);
    }

    public async Task<Result<Client>> GetClient(string brokerId)
    {
        var brokerTask = _brokerService.GetBroker(brokerId);
        var deviceTask = _brokerService.GetBrokerDevices(brokerId);

        await Task.WhenAll(brokerTask, deviceTask);

        var brokerResult = brokerTask.Result;
        var deviceResult = deviceTask.Result;

        if (!brokerResult.Succeeded)
            return Result<Client>.Fail(brokerResult.StatusCode, brokerResult.Messages);

        if (!deviceResult.Succeeded)
            return Result<Client>.Fail(deviceResult.StatusCode, deviceResult.Messages);

        var client = _clients.FirstOrDefault(x => x.Id == brokerId);

        if (client is null)
        {
            var newClient = new Client(brokerResult.Data, _topicService, _factory);

            var failedConnections = await UpdateClientDevices(newClient, deviceResult.Data);

            _clients.Add(newClient);

            if (failedConnections == 0)
                return (Result<Client>)Result<Client>.Success();

            return Result<Client>.Fail($"Failed to subsribe {failedConnections} topics.");
        }
        else if (client.Broker.EditedAt != brokerResult.Data.EditedAt)
        {
            await UpdateClientBroker(client, brokerResult.Data);
        }

        var failConnections = await UpdateClientDevices(client, deviceResult.Data);

        if (failConnections == 0)
            return (Result<Client>)Result<Client>.Success();

        return Result<Client>.Fail($"Failed to subsribe {failConnections} topics.");
    }

    public async Task<Result<Client>> UpdateClient(Broker broker)
    {
        var result = await _brokerService.UpdateBroker(broker);

        if (result.Succeeded)
        {
            var client = _clients.FirstOrDefault(x => x.Id == broker.Id)!;

            await UpdateClientBroker(client, result.Data);

            return Result<Client>.Success(result.StatusCode, client);
        }

        return Result<Client>.Fail(result.StatusCode, result.Messages);
    }

    public async Task<Result> RemoveClient(string brokerId)
    {
        var result = await _brokerService.RemoveBroker(brokerId);

        if(result.Succeeded)
        {
            var client = _clients.First(x => x.Broker.Id == brokerId);
            await client.DisposeAsync();
            _clients.Remove(client);

            return Result.Success(result.StatusCode);
        }

        return Result.Fail(result.StatusCode, result.Messages);
    }

    public async Task<Result<Client>> CreateClient(Broker broker)
    {
        var result = await _brokerService.CreateBroker(broker);

        if(result.Succeeded)
        {
            var client = new Client(result.Data, _topicService, _factory);
            _clients.Add(client);

            return Result<Client>.Success(result.StatusCode, client);
        }

        return Result<Client>.Fail(result.StatusCode, result.Messages);
    }

    #region Device

    public async Task<Result> RemoveDeviceFromClient(string clientId, Device device)
    {
        var result = await _deviceService.RemoveDevice(clientId);

        if (result.Succeeded)
        {
            var client = _clients.First(x => x.Id == clientId);
            await client.DisconnectAsync(device);
            client.Devices.RemoveAll(x=>x.Id == device.Id);

            return Result.Success(result.StatusCode);
        }

        return Result.Fail(result.StatusCode, result.Messages);
    }

    public async Task<Result<Device>> CreateDeviceForClient(Device device)
    {
        var result = await _deviceService.CreateDevice(device);

        if (result.Succeeded)
        {
            var client = _clients.First(x => x.Id == device.BrokerId);
            client.Devices.Add(result.Data);

            return (Result<Device>) result;
        }

        return Result<Device>.Fail(result.StatusCode, result.Messages);
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

            return Result<Device>.Fail($"Failed to subsribe {unsuccessfullConnections} topics.");
        }

        return Result<Device>.Fail(result.StatusCode, result.Messages);
    }

    #endregion

    #region Privates

    private async Task<int> UpdateClientDevices(Client client, List<Device> devices)
    {
        HashSet<string> usedDevices = new();

        int invalidConnectionCount = 0;

        foreach(var device in devices)
        {
            var existingDevice = client.Devices.FirstOrDefault(x => x.Id == device.Id);
            if(existingDevice is null)
            {
                var result = await _deviceService.GetDeviceControls(device.Id);

                if (result.Succeeded)
                {
                    if(await client.SubscribeAsync(device, result.Data))
                        invalidConnectionCount+=1;
                    device.SuccessfullControlsFetch = true;
                }
                else
                {
                    device.SuccessfullControlsFetch = false;
                }

            }
            else if(device.EditedAt != existingDevice.EditedAt)
            {
                await client.DisconnectAsync(existingDevice);

                var result = await _deviceService.GetDeviceControls(device.Id);

                if (result.Succeeded)
                {
                    if (await client.SubscribeAsync(device, result.Data))
                        invalidConnectionCount+=1;
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
                    if (await client.UpdateSubscribtionsAsync(device, result.Data))
                        invalidConnectionCount+=1;

                    device.SuccessfullControlsFetch = true;
                }
                else
                {
                    device.SuccessfullControlsFetch = false;
                } 
            }

            usedDevices.Add(device.Id);
        }

        foreach(var device in client.Devices)
            if(!usedDevices.Contains(device.Id))
                client.Devices.Remove(device);

        return invalidConnectionCount;
    }

    private async Task UpdateClientBroker(Client client, Broker broker)
    {
        var connected = client.IsConnected;

        if (connected)
            await client.DisconnectAsync();

        client.Broker = broker;

        if (connected)
            await client.ConnectAsync();
    }

    #endregion
}
