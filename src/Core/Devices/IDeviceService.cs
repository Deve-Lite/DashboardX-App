namespace Core.Devices;

public interface IDeviceService
{
    Task<IResult<List<Device>>> GetDevices();
    Task<IResult<List<Device>>> GetDevices(string brokerId);
    Task<IResult<Device>> GetDevice(string id);
    Task<IResult<Device>> CreateDevice(DeviceDTO broker);
    Task<IResult<Device>> UpdateDevice(DeviceDTO broker);
    Task<IResult> RemoveDevice(string deviceId);

    Task<IResult<List<Control>>> GetDeviceControls(string deviceId);
    Task<IResult> RemoveDeviceControls(string deviceId, string controlId);
    Task<IResult<Control>> CreateDeviceControl(Control control);
    Task<IResult<Control>> UpdateDeviceControl(Control control);
}
