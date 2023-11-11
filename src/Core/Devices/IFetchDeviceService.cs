namespace Core.Devices;

public interface IFetchDeviceService
{
    Task<IResult<List<Device>>> GetDevices();
    Task<IResult<List<Device>>> GetDevices(string brokerId);
    Task<IResult<Device>> GetDevice(string id);
    Task<IResult<Device>> CreateDevice(DeviceDTO broker);
    Task<IResult<Device>> UpdateDevice(DeviceDTO broker);
    Task<IResult> RemoveDevice(string deviceId);
}
