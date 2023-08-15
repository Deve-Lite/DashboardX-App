using Shared.Models.Devices;

namespace Core.Interfaces;

public interface IDeviceService
{
    Task<IResult<Device>> GetDevice(string id);
    Task<IResult<List<Device>>> GetDevices();
    Task<IResult<Device>> CreateDevices(Device broker);
    Task<IResult<Device>> UpdateDevice(Device broker);
    Task<IResult> DeleteDevice(string deviceId);
}
