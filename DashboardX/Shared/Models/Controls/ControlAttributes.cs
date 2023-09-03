namespace Shared.Models.Controls;

public class ControlAttributes
{
    // Out controls: Button

    //Used by: Button
    [JsonPropertyName("payload")]
    public string Payload { get; set; } = string.Empty;

    // In Controls: Text

    //Two way controls: Switch, Slider, Color, DateTime, State, Radio

    //Used by: Radio

    [JsonPropertyName("payloads")]
    public Dictionary<string, string> Payloads { get; set; } = new();

    // Used by: Switch
    [JsonPropertyName("onPayload")]
    public string OnPayload { get; set; } = string.Empty;

    // Used by: Switch
    [JsonPropertyName("offpayload")]
    public string OffPayload { get; set; } = string.Empty;

    // Used by: Slider, Color, DateTime
    [JsonPropertyName("payloadTemplate")]
    public string PayloadTemplate { get; set; } = string.Empty;

    // Used by: Color 
    [JsonPropertyName("colorFormat")]
    public string ColorFormat { get; set; } = string.Empty;

    // Used by: State
    [JsonPropertyName("secondSpan")]
    public int? SecondSpan { get; set; } = null;

    // Used by: DateTime
    [JsonPropertyName("sendAsTicks")]
    public bool? SendAsTicks { get; set; } = null;

    public ControlAttributes Copy()
    {
        return new()
        {
            Payload = Payload,
            Payloads = new Dictionary<string, string>(Payloads),
            OnPayload = OnPayload,
            OffPayload = OffPayload,
            PayloadTemplate = PayloadTemplate,
            ColorFormat = ColorFormat,
            SecondSpan = SecondSpan,
            SendAsTicks = SendAsTicks
        };
    }
}
