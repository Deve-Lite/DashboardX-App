
using Shared.Models.Controls;

namespace Shared.Models.Devices;

public class Device : BaseModel
{
    [JsonPropertyName("name"), Required, MinLength(2), MaxLength(64)]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("placing"), Required, MinLength(2), MaxLength(64)]
    public string Placing { get; set; } = string.Empty;

    [JsonPropertyName("icon")]
    public string Icon { get; set; } = "default.png";

    [JsonPropertyName("iconBackgroundColor")]
    public string IconBackgroundColor { get; set; } = string.Empty;

    [JsonPropertyName("brokerId"), Required]
    public string BrokerId { get; set; } = string.Empty;

    [JsonPropertyName("baseDevicePath"), Required, MinLength(2), MaxLength(128)]
    public string BaseDevicePath { get; set; } = string.Empty;

    [JsonPropertyName("updatedAt")]
    public DateTime EditedAt { get; set; }

    [JsonIgnore]
    public List<Control> Controls { get; set; } = new();

    [JsonIgnore]
    public bool SuccessfullControlsFetch { get; set; } = true;

    public Device Copy() => new()
    {
        BaseDevicePath = BaseDevicePath,
        BrokerId = BrokerId,
        EditedAt = EditedAt,
        Icon = Icon,
        IconBackgroundColor = IconBackgroundColor,
        Id = Id,
        Name = Name,
        Placing = Placing
    };
}
