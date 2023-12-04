namespace Presentation.Devices;

public class UnusedDeviceService : IUnusedDeviceService
{
    private List<Device> _unusedDevices = new();

    public List<Device> GetUnusedDevices()
    {
        return _unusedDevices;
    }

    public void RemoveDevice(string deviceId)
    {
        _unusedDevices.RemoveAll(x => x.Id == deviceId);
    }

    public void UpdateUnusedDevices(List<Device> devices)
    {
        try
        {
            var pairs = _unusedDevices.Select(x => (x, devices.FirstOrDefault(y => y.Id == x.Id)))
    .Where(x => x.Item2 != null)
    .Select(x => x.Item2!)
    .ToDictionary(x => x.Id);

            foreach (var device in devices)
                if (!pairs.ContainsKey(device.Id))
                    pairs[device.Id] = device;

            _unusedDevices = pairs.ToList()
                                  .Select(x => x.Value)
                                  .ToList();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }
}