
namespace Shared.Models.Controls;

public class ControlDTO : Control
{
    [JsonPropertyName("attributes")]
    public AttributesDTO Attributes { get; set; } = new();
}
