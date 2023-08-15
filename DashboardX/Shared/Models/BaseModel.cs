

namespace Shared.Models;

public class BaseModel : IIdentifiedEntity
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;
}
