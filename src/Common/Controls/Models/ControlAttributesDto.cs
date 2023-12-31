namespace Common.Controls.Models;

public class ControlAttributesDto
{
    [JsonPropertyName("payload")]
    public string? Payload { get; set; }

    [JsonPropertyName("payloads")]
    public Dictionary<string, string>? Payloads { get; set; }

    [JsonPropertyName("onPayload")]
    public string? OnPayload { get; set; }

    [JsonPropertyName("offPayload")]
    public string? OffPayload { get; set; }

    [JsonPropertyName("payloadTemplate")]
    public string? PayloadTemplate { get; set; }

    [JsonPropertyName("maxValue")]
    public double? MaxValue { get; set; }

    [JsonPropertyName("minValue")]
    public double? MinValue { get; set; }

    [JsonPropertyName("colorFormat")]
    public string? StringColorFormat { get; set; }

    [JsonPropertyName("sendAsTicks")]
    public bool? SendAsTicks { get; set; } = null;

    public static ControlAttributesDto Button(string payload)
    {
        return new ControlAttributesDto
        {
            Payload = payload
        };
    }

    public static ControlAttributesDto Color(string payloadTemplate, ColorFormat colorFormat)
    {
        return new ControlAttributesDto
        {
            PayloadTemplate = payloadTemplate,
            StringColorFormat = colorFormat.ToString()
        };
    }

    public static ControlAttributesDto DateTime(bool sendAsTicks, string payloadTemplate)
    {
        return new ControlAttributesDto
        {
            SendAsTicks = sendAsTicks,
            PayloadTemplate = payloadTemplate
        };
    }

    public static ControlAttributesDto Radio(Dictionary<string, string> payloads)
    {
        return new ControlAttributesDto
        {
            Payloads = payloads
        };
    }

    public static ControlAttributesDto Slider(string payloadTemplate, double minValue, double maxValue)
    {
        return new ControlAttributesDto
        {
            PayloadTemplate = payloadTemplate,
            MinValue = minValue,
            MaxValue = maxValue
        };
    }

    public static ControlAttributesDto State(string onPayload, string offPayload)
    {
        return new ControlAttributesDto
        {
            OnPayload = onPayload,
            OffPayload = offPayload
        };
    }

    public static ControlAttributesDto Switch(string onPayload, string offPayload)
    {
        return new ControlAttributesDto
        {
            OnPayload = onPayload,
            OffPayload = offPayload
        };
    }

    public static ControlAttributesDto Text()
    {
        return new ControlAttributesDto();
    }

    public ControlAttributes Model()
    {
        return new ControlAttributes
        {
            Payload = Payload,
            Payloads = Payloads,
            OnPayload = OnPayload,
            OffPayload = OffPayload,
            PayloadTemplate = PayloadTemplate,
            MaxValue = MaxValue,
            MinValue = MinValue,
            StringColorFormat = StringColorFormat,
            SendAsTicks = SendAsTicks ?? false
        };
    }
}
