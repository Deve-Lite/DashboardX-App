using DashboardXModels.Brokers;

namespace DashboardX.Devices;

public interface IDeviceService
{
    Task<Response<Broker>> GetDevice(string id);
    Task<Response<List<Broker>>> GetDevices(string brokerId);
    Task<Response<List<Broker>>> GetDevices();
    Task<Response<Broker>> CreateDevices(Broker broker);
    Task<Response<Broker>> UpdateDevices(Broker broker);
    Task<Response> DeleteDevices(string id);
}
