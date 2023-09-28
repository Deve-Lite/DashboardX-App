namespace Common;

public class Icon
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
    [JsonPropertyName("backgroundColor")]
    public string BackgroundHex { get; set; } = string.Empty;

    public Icon Copy() => new()
    {
        Name = Name,
        BackgroundHex = BackgroundHex
    };
}
