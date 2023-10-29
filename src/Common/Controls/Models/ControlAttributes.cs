namespace Common.Controls.Models;

public class ControlAttributes
{
    // Out controls: Button, DateTime

    //Used by: Button
    [JsonPropertyName("payload")]
    public string? Payload { get; set; }

    // In Controls: Text

    //Two way controls: Switch, Slider, Color, State, Radio

    //Used by: Radio

    [JsonPropertyName("payloads")]
    public Dictionary<string, string>? Payloads { get; set; }

    // Used by: Switch
    [JsonPropertyName("onPayload")]
    public string? OnPayload { get; set; }

    // Used by: Switch
    [JsonPropertyName("offPayload")]
    public string? OffPayload { get; set; }

    // Used by: Slider, Color, DateTime
    [JsonPropertyName("payloadTemplate")]
    public string? PayloadTemplate { get; set; }

    // Used by: Slider
    [JsonPropertyName("maxValue")]
    public double? MaxValue { get; set; }

    // Used by: Slider
    [JsonPropertyName("minValue")]
    public double? MinValue { get; set; }

    // Used by: Color 
    [JsonPropertyName("colorFormat")]
    public string? StringColorFormat { get; set; }

    [JsonIgnore]
    public ColorFormat ColorFormatEnum
    {
        get
        {
            return StringColorFormat switch
            {
                "rgb" => ColorFormat.HexRGB,
                _ => ColorFormat.HexRGB,
            };
        }
        set
        {
            StringColorFormat = value switch
            {
                ColorFormat.HexRGB => "rgb",
                _ => "rgb"
            };
        }
    }

    // Used by: DateTime
    [JsonPropertyName("sendAsTicks")]
    public bool? SendAsTicks { get; set; } = null;

    // Used by: State
    [JsonPropertyName("secondSpan")]
    public int? SecondSpan { get; set; } = null;

    public ControlAttributes Copy()
    {
        var payloads = Payloads == null ? null : new Dictionary<string, string>(Payloads);

        return new()
        {
            Payload = Payload,
            Payloads = payloads,
            OnPayload = OnPayload,
            OffPayload = OffPayload,
            PayloadTemplate = PayloadTemplate,
            StringColorFormat = StringColorFormat,
            SecondSpan = SecondSpan,
            SendAsTicks = SendAsTicks,
            MaxValue = MaxValue,
            MinValue = MinValue
        };
    }
}
