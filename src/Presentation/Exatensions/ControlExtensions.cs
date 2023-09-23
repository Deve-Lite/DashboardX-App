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
            case ControlType.State:
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
                var key = additionalValue as string;
                var radioPayload = control.Attributes.Payloads!.GetValueOrDefault(key, string.Empty);
                await client.PublishAsync(topic, radioPayload, control.QualityOfService);
                break;
            case ControlType.Slider:
                var value = additionalValue as string;
                var sliderPayload = control.Attributes.PayloadTemplate;
                sliderPayload = sliderPayload.Replace("!value!", value);
                await client.PublishAsync(topic, sliderPayload, control.QualityOfService);
                break;
            case ControlType.DateTime:
                var dateTime = additionalValue as DateTime?;
                var stringValue = dateTime.ToString();

                if (control.Attributes.SendAsTicks!.Value)
                    stringValue = dateTime!.Value.Ticks.ToString();

                var dateTimePayload = control.Attributes.PayloadTemplate.Replace("!value!", stringValue);
                await client.PublishAsync(topic, dateTimePayload, control.QualityOfService);
                break;
            case ControlType.Switch:
                var isOn = additionalValue as bool?;
                var switchPayload = isOn!.Value ? control.Attributes.OnPayload : control.Attributes.OffPayload;
                await client.PublishAsync(topic, switchPayload, control.QualityOfService);
                break;
            case ControlType.Color:
                //TODO: to think of possibilites
                var color = additionalValue as string;
                var colorPayload = control.Attributes.PayloadTemplate.Replace("!value!", color);
                await client.PublishAsync(topic, "", control.QualityOfService);
                break;
        }
    }
}
