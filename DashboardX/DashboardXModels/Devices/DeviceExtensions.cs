namespace DashboardXModels.Devices;

public static class DeviceExtensions
{
    public static IEnumerable<Control> GetControls(this Device device)
    {
        var controls = new List<Control>();

        controls.AddRange(device.SwitchControls);
        controls.AddRange(device.SliderControls);
        controls.AddRange(device.RadioControls);
        controls.AddRange(device.ButtonControls);
        controls.AddRange(device.ColorControls);
        controls.AddRange(device.TextOutControls);
        controls.AddRange(device.TimeAndDateControls);
        controls.AddRange(device.StateControls);

        return controls;
    }
}
