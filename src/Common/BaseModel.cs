namespace Common;

public class BaseModel : IIdentifiedEntity
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;
}
