namespace DashboardXModels.Devices;

public static class DeviceExtensions
{
    public static DateTime CreatedAt(this Device device) => new(device.CreatedAtTicks);

    public static DateTime EditedAt(this Device device) => new(device.EditedAtTicks);
}
