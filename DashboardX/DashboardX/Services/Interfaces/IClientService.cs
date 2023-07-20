using DashboardXModels.Brokers;
using DashboardXModels.Devices;

namespace DashboardX.Services.Interfaces;

public interface IClientService
{
    Task Initialize();
    Task<List<Broker>> GetBrokers();
    Task<Broker> GetBroker(string id);
    Task<IEnumerable<Device>> GetDevices(string brokerId);
    Task<IEnumerable<Device>> GetDevice(string deviceId);

}
