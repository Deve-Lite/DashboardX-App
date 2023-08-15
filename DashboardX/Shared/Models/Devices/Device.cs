
namespace Shared.Models.Devices;

public class Device : BaseModel
{
    public string Name { get; set; } = string.Empty;
    public string Placing { get; set; } = string.Empty;

    public string Icon { get; set; } = string.Empty;
    public string IconBackgroundColor { get; set; } = string.Empty;

    public string BrokerId { get; set; } = string.Empty;
    public string BaseDevicePath { get; set; } = string.Empty;

    public DateTime EditedAt { get; set; }
}
