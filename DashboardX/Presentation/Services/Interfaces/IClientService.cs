using Infrastructure;
using Presentation.Models;

namespace Presentation.Services.Interfaces; 

public interface IClientService
{
    Task<Result<List<BrokerClient>>> GetBrokers();
    Task<Result<List<BrokerClient>>> GetBroker(string brokerId);

    Task<Result<List<DeviceClient>>> GetDevices();
    Task<Result<List<DeviceClient>>> GetDevices(string brokerId);
    Task<Result<DeviceClient>> GetDevice(string clientId);
}

