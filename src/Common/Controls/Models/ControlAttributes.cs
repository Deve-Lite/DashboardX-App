﻿namespace Common.Controls.Models;

public class ControlAttributes
{
    // Out controls: Button

    //Used by: Button
    [JsonPropertyName("payload")]
    public string? Payload { get; set; }

    // In Controls: Text

    //Two way controls: Switch, Slider, Color, DateTime, State, Radio

    //Used by: Radio

    [JsonPropertyName("payloads")]
    public Dictionary<string, string>? Payloads { get; set; }

    // Used by: Switch
    [JsonPropertyName("onPayload")]
    public string? OnPayload { get; set; }

    // Used by: Switch
    [JsonPropertyName("offpayload")]
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
                "rgb" => ColorFormat.RGB,
                _ => ColorFormat.RGB,
            };
        }
        set
        {
            StringColorFormat = value switch
            {
                ColorFormat.RGB => "rgb",
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
        return new()
        {
            Payload = Payload,
            Payloads = new Dictionary<string, string>(Payloads),
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
