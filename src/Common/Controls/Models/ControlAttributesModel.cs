namespace Common.Controls.Models;

public class ControlAttributesModel
{
    public ControlType Type { get; set; } = ControlType.Button;

    public string Payload { get; set; } = string.Empty;
    public string OnPayload { get; set; } = string.Empty;
    public string OffPayload { get; set; } = string.Empty;
    public string PayloadTemplate { get; set; } = string.Empty;

    public double MaxValue { get; set; }
    public double MinValue { get; set; }

    public Dictionary<string, string> Payloads { get; set; } = new();
    public ColorFormat ColorFormat { get; set; }
    public bool SendAsTicks { get; set; }

    public ControlAttributesModel()
    {
        Type = ControlType.Text;
    }

    public ControlAttributesModel(ControlType type)
    {
        Type = type;

        switch (type)
        {
            case ControlType.Slider:
                PayloadTemplate = string.Empty;
                MinValue = 0;
                MaxValue = 100;
                break;
            case ControlType.Switch or ControlType.State:
                OnPayload = string.Empty;
                OffPayload = string.Empty;
                break;
            case ControlType.Button:
                Payload = string.Empty;
                break;
            case ControlType.Radio:
                Payloads = new();
                break;
            case ControlType.DateTime:
                SendAsTicks = false;
                PayloadTemplate = string.Empty;
                break;
            case ControlType.Color:
                PayloadTemplate = string.Empty;
                ColorFormat = ColorFormat.HexRGB;
                break;
            default:
                break;
        }
    }

    public ControlAttributesDto Attributes()
    {
        switch(Type)
        {
            case ControlType.Button:
                return ControlAttributesDto.Button(Payload);
            case ControlType.Color:
                return ControlAttributesDto.Color(PayloadTemplate, ColorFormat);
            case ControlType.DateTime:
                return ControlAttributesDto.DateTime(SendAsTicks, PayloadTemplate);
            case ControlType.Radio:
                return ControlAttributesDto.Radio(Payloads);
            case ControlType.Slider:
                return ControlAttributesDto.Slider(PayloadTemplate, MinValue, MaxValue);
            case ControlType.State:
                return ControlAttributesDto.State(OnPayload, OffPayload);
            case ControlType.Switch:
                return ControlAttributesDto.Switch(OnPayload, OffPayload);
            case ControlType.Text:
                return ControlAttributesDto.Text();
            default:
                throw new InvalidOperationException($"Unknown control type: {Type}");
        }
    }
}
