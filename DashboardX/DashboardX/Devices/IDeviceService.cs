using DashboardXModels.Devices;

namespace DashboardX.Devices;

public interface IDeviceService
{
    Task<Response<Device>> GetDevice(string id);
    Task<Response<List<Device>>> GetDevices(string brokerId);
    Task<Response<List<Device>>> GetDevices();
    Task<Response<Device>> CreateDevices(Device broker);
    Task<Response<Device>> UpdateDevices(Device broker);
    Task<Response> DeleteDevices(string id);
}
