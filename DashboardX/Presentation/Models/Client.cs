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
    public async Task DisconnectAsync(Device device)
    {
        foreach (var control in device.Controls)
        {
            var topic = await _topicService.RemoveTopic(Broker.Id, device, control);
            await Service.UnsubscribeAsync(topic);
        }

        Devices.Remove(device);
    }
    public async Task DisconnectAsync(string deviceId, Control control)
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
        {
            try
            {
                var topic = await _topicService.AddTopic(Broker.Id, device, control);

                if (!Service.IsConnected)
                    await ConnectAsync();

                await Service.SubscribeAsync(topic, control.QualityOfService);
                device.Controls.Add(control);
            }
            catch
            {
                ++failedSubscribtions;
            }
        }

        return failedSubscribtions;
    }

    /// <summary>
    /// Updates subsribion topics for a device. 
    /// </summary>
    /// <param name="existingDevice"> Device must exist in collection of elemnts.</param>
    /// <param name="controls"></param>
    /// <returns></returns>
    public async Task<int> UpdateSubscribtionsAsync(Device existingDevice, List<Control> controls)
    {
        HashSet<string> usedControls = new();

        int failedSubscribtions = 0;

        foreach (var control in controls)
        {
            try
            {
                var existingControl = existingDevice.Controls.FirstOrDefault(x => x.Id == control.Id);

                if (existingControl is null || existingControl.IsTheSame(control))
                {
                    var topic = await _topicService.AddTopic(Broker.Id, existingDevice, control);

                    if (!Service.IsConnected)
                        await ConnectAsync();

                    await Service.SubscribeAsync(topic);
                    existingDevice.Controls.Add(control);
                }

                usedControls.Add(control.Id);
            }
            catch
            {
                ++failedSubscribtions;
            }
        }

        foreach (var control in existingDevice.Controls)
            if (!usedControls.Contains(control.Id))
            {
                var topic = await _topicService.RemoveTopic(Broker.Id, existingDevice, control);
                await Service.UnsubscribeAsync(topic);
                existingDevice.Controls.Remove(control);
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
        var optionsBuilder = _factory.CreateClientOptionsBuilder()
            .WithClientId(Broker.ClientId)
            .WithWebSocketServer($"wss://{Broker.Server}:{Broker.Port}/mqtt");

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
    }

    #endregion
}
