using Presentation.Models;
using Shared.Models.Controls;
using Shared.Models.Devices;

namespace Presentation.Extensions;

public static class ControlExtensions
{
    public static async Task Send(this Control control, Device device, Client client)
    {
        var topic = control.GetTopic(device);

        switch (control.Type)
        {
            case ControlType.Button:
                await client.PublishAsync(topic, control.Attributes.Payload, control.QualityOfService);
                break;
        }
    }

    public static async Task Send<T>(this Control control, Device device, Client client, T additionalValue)
    {
        var topic = control.GetTopic(device);

        switch (control.Type)
        {
            case ControlType.Radio:
                var key = (additionalValue as string);
                var payload = control.Attributes.Payloads!.GetValueOrDefault(key, string.Empty);
                await client.PublishAsync(topic, payload, control.QualityOfService);
                break;
            case ControlType.Slider:
                var value = (additionalValue as string);
                var payloadTemplate = control.Attributes.PayloadTemplate;
 
                payloadTemplate = payloadTemplate.Replace("!value!", value);

                await client.PublishAsync(topic, payloadTemplate, control.QualityOfService);

                break;
        }
    }
}
