namespace Presentation.Devices.Interfaces;

public interface IUnusedDeviceService
{
    List<Device> GetUnusedDevices();
    void UpdateUnusedDevices(IList<Device> devices);
    void RemoveDevice(string deviceId);
    bool ContainsDevice(string deviceId);
}