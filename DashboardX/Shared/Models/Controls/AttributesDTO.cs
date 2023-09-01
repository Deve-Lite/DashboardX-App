

namespace Shared.Models.Controls;

public class AttributesDTO
{
    // Button
    [JsonPropertyName("payload")]
    public string Payload { get; set; } = string.Empty;

    // Color
    [JsonPropertyName("payloadTemplate")]
    public string PayloadTemplate { get; set; } = string.Empty;
    [JsonPropertyName("value")]
    public string Value { get; set; } = string.Empty;
    [JsonPropertyName("colorFormat")]
    public string ColorFormat { get; set; } = string.Empty;

    // DateTime

    [JsonPropertyName("sendAsTicks")]
    public bool SendAsTicks { get; set; } = false;

    // MultiButton - To Rethink

    //uses payload as tempalte ??

    [JsonPropertyName("nameToVlaues")]
    public List<string> Properties { get; set; } = new();

    // Radio

    [JsonPropertyName("payloads")]
    public List<string> Payloads { get; set; } = new();

    // Slider

    //uses string value - TODO Resolve mismatch 

    // State

    //uses payload
    //as delay
    [JsonPropertyName("secondSpan")]
    public int SecondSpan { get; set; }

    // Switch
    [JsonPropertyName("onPayload")]
    public string OnPayload { get; set; } = string.Empty;

    [JsonPropertyName("offpayload")]
    public string OffPayload { get; set; } = string.Empty;

    [JsonPropertyName("state")]
    public bool State { get; set; } = false;  

    // TextOut

}
