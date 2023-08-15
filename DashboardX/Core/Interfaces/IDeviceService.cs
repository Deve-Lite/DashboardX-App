using Shared.Models.Devices;

namespace Core.Interfaces;

public interface IDeviceService
{
    Task<IResult<Device>> GetDevice(string id);
    Task<IResult<List<Device>>> GetDevices();
    Task<IResult> CreateDevices(Device broker);
    Task<IResult> UpdateDevice(Device broker);
    Task<IResult> DeleteDevice(string deviceId);
}
