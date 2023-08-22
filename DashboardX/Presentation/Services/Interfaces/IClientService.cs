using Infrastructure;
using Presentation.Models;
using Shared.Models.Brokers;
using Shared.Models.Devices;

namespace Presentation.Services.Interfaces; 

public interface IClientService
{
    Task<Result<List<Client>>> GetClients();
    Task<Result<Client>> GetClient(string brokerId);
    Task<Result> RemoveClient(string brokerId);
    Task<Result<Client>> CreateClient(Broker broker);
    Task<Result<Client>> UpdateClient(Broker broker);

    Task<Result> RemoveDeviceFromClient(string clientId, string deviceId);
    Task<Result<Device>> CreateDeviceForClient(string clientId, Device device);
}

