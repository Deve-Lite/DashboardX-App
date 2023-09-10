using Infrastructure;
using Presentation.Models;
using Shared.Models.Brokers;
using Shared.Models.Controls;
using Shared.Models.Devices;

namespace Presentation.Services.Interfaces; 

public interface IClientService
{
    Task Logout();

    Task<Result<List<Client>>> GetClientsWithDevices();
    Task<Result<List<Client>>> GetClients();
    Task<Result<Client>> GetClient(string clientId);
    Task<Result> RemoveClient(string clientId);
    Task<Result<Client>> CreateClient(Broker broker);
    Task<Result<Client>> UpdateClient(Broker broker);

    Task<Result> RemoveDeviceFromClient(string clientId, Device device);
    Task<Result<Device>> CreateDeviceForClient(Device device);
    Task<Result<Device>> UpdateDeviceForClient(Device device);

    Task<Result> RemoveControlFromDevice(string clientId, string deviceId, Control control);
    Task<Result> CreateControlForDevice(string clientId, string deviceId, Control control);
    Task<Result> UpdateControlForDevice(string clientId, string deviceId, Control control);
}

