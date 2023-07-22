
namespace DashboardXModels.Controls;

public static class ControlExtensions
{
    public static string GetTopic(this Control control, Device device)
    {
        return $"{device.BaseDevicePath}{control.Topic}";
    }
}
