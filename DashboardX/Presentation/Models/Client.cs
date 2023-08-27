﻿using Core.Interfaces;
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

    public async Task<MqttClientConnectResult> ConnectAsync() => await Service.ConnectAsync(Options());
    public async Task DisconnectAsync() => await Service.DisconnectAsync();
    public async Task DisconnectAsync(Device device)
    {
        foreach(var control in device.Controls)
        {
            var topic = await _topicService.RemoveTopic(Broker.Id, device, control);
            await Service.UnsubscribeAsync(topic);
        }

        Devices.Remove(device);
    }

    public async Task SubscribeAsync(Device device, List<Control> controls)
    {
        Devices.Add(device);

        foreach(var control in controls)
        {
            var topic = await _topicService.AddTopic(Broker.Id, device, control);
            await Service.SubscribeAsync(topic, control.QualityOfService);
            device.Controls.Add(control);
        }
    }

    /// <summary>
    /// Updates subsribion topics for a device. 
    /// </summary>
    /// <param name="existingDevice"> Device must exist in collection of elemnts.</param>
    /// <param name="controls"></param>
    /// <returns></returns>
    public async Task UpdateSubscribtionsAsync(Device existingDevice, List<Control> controls)
    {
        HashSet<string> usedControls = new();

        foreach (var control in controls)
        {
            var existingControl = existingDevice.Controls.FirstOrDefault(x => x.Id == control.Id);

            if(existingControl is null || existingControl.IsTheSame(control))
            {
                var topic = await _topicService.AddTopic(Broker.Id, existingDevice, control);
                await Service.SubscribeAsync(topic);
                existingDevice.Controls.Add(control);
            }
            
            usedControls.Add(control.Id);
        }

        foreach (var control in existingDevice.Controls)
            if(!usedControls.Contains(control.Id))
            {
                var topic = await _topicService.RemoveTopic(Broker.Id, existingDevice, control);
                await Service.UnsubscribeAsync(topic);
                existingDevice.Controls.Remove(control);
            }
    }

    public async ValueTask DisposeAsync()
    {
        foreach(var device in Devices)
            foreach(var control in device.Controls)
                await _topicService.RemoveTopic(Broker.Id, device, control);

        await Service.DisconnectAsync();
        Service.Dispose();
    }

    #region Privates

    private MqttClientOptions Options()
    {
        var optionsBuilder = _factory.CreateClientOptionsBuilder()
            .WithClientId(Broker.ClientId)
            .WithWebSocketServer($"wss://{Broker.Server}:{Broker.Port}");

        if (!string.IsNullOrEmpty(Broker.Username) && !string.IsNullOrEmpty(Broker.Password))
            optionsBuilder = optionsBuilder.WithCredentials(Broker.Username, Broker.Password);

        return optionsBuilder.Build();
    }

    private void InitializeCallbacks(ITopicService topicService)
    {
        Service.ApplicationMessageReceivedAsync += (e) =>
        {
            var topic = e.ApplicationMessage.Topic;
            var message = Encoding.UTF8.GetString(e.ApplicationMessage.PayloadSegment);
            topicService.UpdateMessageOnTopic(Broker.Id, topic, message);

            return Task.CompletedTask;
        };
    }

    #endregion
}
