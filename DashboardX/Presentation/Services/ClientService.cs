using Core.Interfaces;
using Infrastructure;
using MQTTnet;
using Presentation.Models;
using Presentation.Services.Interfaces;
using Shared.Models.Brokers;
using Shared.Models.Devices;
using System.Net;

namespace Presentation.Services;

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

    public Task<Result<List<Client>>> GetClientsWithDevices()
    {
        throw new NotImplementedException();
    }

    public async Task<Result<List<Client>>> GetClients()
    {
        var result = await _brokerService.GetBrokers();

        if (!result.Succeeded)
            return Result<List<Client>>.Fail(result.StatusCode, result.Messages);

        if (result.StatusCode == HttpStatusCode.NotModified)
            return Result<List<Client>>.Success(result.StatusCode, _clients);

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
        var brokerResult = await _brokerService.GetBroker(brokerId);
        var deviceResult = await _brokerService.GetBrokerDevices(brokerId);

        if (!brokerResult.Succeeded)
            return Result<Client>.Fail(brokerResult.StatusCode, brokerResult.Messages);

        if (!deviceResult.Succeeded)
            return Result<Client>.Fail(deviceResult.StatusCode, deviceResult.Messages);

        var client = _clients.FirstOrDefault(x => x.Id == brokerId);

        if (client is null)
        {
            var newClient = new Client(brokerResult.Data, _topicService, _factory);

            await UpdateClientDevices(newClient, deviceResult.Data);

            _clients.Add(newClient);

            return Result<Client>.Success(brokerResult.StatusCode, newClient);
        }
        else if (client.Broker.EditedAt != brokerResult.Data.EditedAt)
        {
            await UpdateClientBroker(client, brokerResult.Data);
        }

        await UpdateClientDevices(client, deviceResult.Data);

        return Result<Client>.Success(brokerResult.StatusCode, client);
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

    public async Task<Result> RemoveDeviceFromClient(string clientId, string deviceId)
    {
        var result = await _deviceService.RemoveDevice(clientId);

        if (result.Succeeded)
        {
            var client = _clients.First(x => x.Id == clientId);
            //TODO: Remove topics from client
            client.Devices.RemoveAll(x=>x.Id == deviceId);

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
            client.Devices.RemoveAll(x => x.Id == device.Id);
            client.Devices.Add(result.Data);

            return (Result<Device>)result;
        }

        return Result<Device>.Fail(result.StatusCode, result.Messages);
    }

    #endregion

    #region Privates

    private Task UpdateDeviceControls()
    {
        throw new NotImplementedException();
    }

    private Task UpdateClientDevices(Client client, List<Device> devices)
    {
        HashSet<string> usedDevices = new();

        foreach(var device in devices)
        {
            var existingDevice = client.Devices.FirstOrDefault(x => x.Id == device.Id);
            if(existingDevice is null)
            {
                client.Devices.Add(device);
                //TODO: Subscribe to device topics
            }
            else if(existingDevice.EditedAt != existingDevice.EditedAt)
            {
                //TODO: Update device
            }

            usedDevices.Add(device.Id);
        }

        foreach(var device in client.Devices)
            if(!usedDevices.Contains(device.Id))
                client.Devices.Remove(device);    

        return Task.CompletedTask;
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
