using Core.Interfaces;
using Microsoft.Extensions.Options;
using MQTTnet;
using MQTTnet.Client;
using Shared.Models.Brokers;
using Shared.Models.Controls;
using Shared.Models.Devices;
using System.Text;

namespace Presentation.Models;

public class Client : IAsyncDisposable
{
    private readonly MqttFactory _factory;
    private readonly ITopicService _topicService;

    public string Id => Broker.Id;
    public bool IsConnected => Service.IsConnected;

    public Broker Broker { get; set; } = new();
    public IMqttClient Service { get; set; }
    public List<Device> Devices { get; set; } = new();

    public Client(Broker broker, ITopicService topicService, MqttFactory factory)
    {
        Broker = broker;
        Service = factory.CreateMqttClient();

        _topicService = topicService;
        _factory = factory;

        InitializeCallbacks(topicService);
    }

    public async Task<MqttClientConnectResult> ConnectAsync()
    {
        try
        {
            var options = Options();
            return await Service.ConnectAsync(options);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return new MqttClientConnectResult();
        }
    }
    public async Task DisconnectAsync() => await Service.DisconnectAsync();
    public async Task UnsubscribeAsync(Device device)
    {
        foreach (var control in device.Controls)
        {
            var topic = await _topicService.RemoveTopic(Broker.Id, device, control);
            await Service.UnsubscribeAsync(topic);
        }

        Devices.Remove(device);
    }
    public async Task UnsubscribeAsync(string deviceId, Control control)
    {
        var device = Devices.First(x => x.Id == deviceId);
        var topic = await _topicService.RemoveTopic(Broker.Id, device, control);
        await Service.UnsubscribeAsync(topic);
        device.Controls.Remove(control);
    }

    public async Task<int> SubscribeAsync(Device device, List<Control> controls)
    {
        Devices.Add(device);
        int failedSubscribtions = 0;

        foreach (var control in controls)
            if(!await SubscribeAsync(device, control))
                ++failedSubscribtions;
            
        return failedSubscribtions;
    }

    public async Task<bool> SubscribeAsync(Device device, Control control)
    {
        try
        {
            if (_topicService.ConatinsTopic(Id, device, control))
                return true;

            var topic = await _topicService.AddTopic(Broker.Id, device, control);

            if (!Service.IsConnected)
                await ConnectAsync();

            device.Controls.Add(control);
            var result = await Service.SubscribeAsync(topic, control.QualityOfService);

            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Updates subsribion topics for a device. 
    /// </summary>
    /// <param name="existingDevice"> Device must exist in collection of elemnts.</param>
    /// <param name="controls"></param>
    /// <returns></returns>
    public async Task<int> UpdateSubscribtionsAsync(string deviceId, List<Control> controls)
    {
        HashSet<string> usedControls = new();

        int failedSubscribtions = 0;

        var device = Devices.First(x => x.Id == deviceId);

        foreach (var control in controls)
        {
            try
            {
                var existingControl = device.Controls.FirstOrDefault(x => x.Id == control.Id);

                var isSync = existingControl?.IsTheSame(control) ?? false;

                if (existingControl is null || !isSync)
                {
                    if(!isSync)
                        await UnsubscribeAsync(device.Id, existingControl!);

                    var topic = await _topicService.AddTopic(Broker.Id, device, control);

                    if (!Service.IsConnected)
                        await ConnectAsync();

                    device.Controls.Add(control);
                    await Service.SubscribeAsync(topic);
                }

                usedControls.Add(control.Id);
            }
            catch
            {
                ++failedSubscribtions;
            }
        }

        foreach (var control in device.Controls)
            if (!usedControls.Contains(control.Id))
            {
                var topic = await _topicService.RemoveTopic(Broker.Id, device, control);
                await Service.UnsubscribeAsync(topic);
                device.Controls.Remove(control);
            }

        return failedSubscribtions;
    }

    public async ValueTask DisposeAsync()
    {
        foreach (var device in Devices)
            foreach (var control in device.Controls)
                await _topicService.RemoveTopic(Broker.Id, device, control);

        await Service.DisconnectAsync();
        Service.Dispose();
    }

    #region Privates

    private MqttClientOptions Options()
    {
        //TODO: Check if celan sesion is not deleting subs if yes every reconec we need to resubscribe to all topics

        var optionsBuilder = _factory.CreateClientOptionsBuilder()
            .WithClientId(Broker.ClientId)
            .WithWebSocketServer($"wss://{Broker.Server}:{Broker.Port}/mqtt")
            .WithCleanSession(false);

        if (!string.IsNullOrEmpty(Broker.Username) && !string.IsNullOrEmpty(Broker.Password))
            optionsBuilder = optionsBuilder.WithCredentials(Broker.Username, Broker.Password);

        return optionsBuilder.Build();
    }

    private void InitializeCallbacks(ITopicService topicService)
    {
        Service.ApplicationMessageReceivedAsync += (e) =>
        {
            var topic = e.ApplicationMessage.Topic;
            var message = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
            topicService.UpdateMessageOnTopic(Broker.Id, topic, message);

            return Task.CompletedTask;
        };

        //TODO: check what happend after disconnect
    }

    #endregion
}
