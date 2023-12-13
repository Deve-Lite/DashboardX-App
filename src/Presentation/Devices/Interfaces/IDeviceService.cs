namespace Presentation.Devices.Interfaces;

public interface IDeviceService
{
    Task<IResult> RemoveDevice(string clientId, string deviceId);
    Task<IResult> CreateDevice(DeviceDTO device);
    Task<IResult> UpdateDevice(DeviceDTO devicee, string oldClientId);
}
