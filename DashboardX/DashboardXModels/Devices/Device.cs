
namespace DashboardXModels.Devices;

public class Device
{
    [Key]
    public string DeviceId { get; set; } = string.Empty;
    public long EditedAtTicks { get; set; }
    public long CreatedAtTicks { get; set; }

    public string Name { get; set; } = string.Empty;
    public string Placing { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public string BrokerId { get; set; } = string.Empty;
    public string BaseDevicePath { get; set; } = string.Empty;

    public IEnumerable<SwitchControl> SwitchControls { get; set; } = new List<SwitchControl>();
    public IEnumerable<SliderControl> SliderControls { get; set; } = new List<SliderControl>();
    public IEnumerable<RadioControl> RadioControls { get; set; } = new List<RadioControl>();
    public IEnumerable<ButtonControl> ButtonControls { get; set; } = new List<ButtonControl>();
    public IEnumerable<ColorControl> ColorControls { get; set; } = new List<ColorControl>();
    public IEnumerable<TextOutControl> TextOutControls { get; set; } = new List<TextOutControl>();
    public IEnumerable<TimeAndDateControl> TimeAndDateControls { get; set; } = new List<TimeAndDateControl>();
    public IEnumerable<StateControl> StateControls { get; set; } = new List<StateControl>();
}
