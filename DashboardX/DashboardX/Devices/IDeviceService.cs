using DashboardXModels.Brokers;
using DashboardXModels.Devices;

namespace DashboardX.Devices;

public interface IDeviceService
{
    Task<Response<Device>> GetDevice(string id);
    Task<Response<List<Device>>> GetDevices(string brokerId);
    Task<Response<List<Device>>> GetDevices();
    Task<Response<Device>> CreateDevices(Broker broker);
    Task<Response<Device>> UpdateDevices(Broker broker);
    Task<Response> DeleteDevices(string id);
}
