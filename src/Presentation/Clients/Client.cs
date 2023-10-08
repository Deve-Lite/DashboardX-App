using Common.Controls.Models;
using Microsoft.IdentityModel.Tokens;
using MQTTnet.Client;
using MQTTnet.Protocol;
using Presentation.Brokers;
using System.Text;

namespace Presentation.Clients;

public class Client : IAsyncDisposable
{
    private readonly ILogger<Client> _logger;
    private readonly MqttFactory _factory;
    public readonly ITopicService TopicService;
    public readonly IMqttClient MqttService;
    public readonly IBrokerService BrokerService;

    public string Id => Broker.Id;
    public bool IsConnected { get; set; }

    public Broker Broker { get; private set; } = new();
    public List<Device> Devices { get; private set; } = new();

    public Func<Task> RerenderPage { get; set; }

    public Client(ILocalStorageService storage, ILogger<Client> clientLogger, IBrokerService brokerService,  MqttFactory factory, Broker broker)
    {
        Broker = broker;
        MqttService = factory.CreateMqttClient();
        BrokerService = brokerService;

        TopicService = new TopicService(storage);

        _factory = factory;
        _logger = clientLogger;

        InitializeCallbacks();
        RerenderPage += () =>
        {
            return Task.CompletedTask;
        };
    }

    public async Task UpdateBroker(Broker broker)
    {
        var connected = MqttService.IsConnected;

        if (connected)
            await DisconnectAsync();

        Broker = broker;

        if (connected)
            await ConnectAsync();
    }

    public async Task<MqttClientPublishResult> PublishAsync(string topic, string payload, MqttQualityOfServiceLevel quality)
    {
        //TODO Change to Result

        try
        {
            if (!MqttService.IsConnected)
                await ConnectAsync();

            var mqttMessage = new MqttApplicationMessageBuilder()
                   .WithTopic(topic)
                   .WithPayload(Encoding.UTF8.GetBytes(payload))
                   .WithQualityOfServiceLevel(quality)
                   .Build();

            return await MqttService.PublishAsync(mqttMessage);
        }
        catch (Exception e)
        {
            _logger.LogError($"Failed to send request {e.Message}");

            return new MqttClientPublishResult();
        }
    }

    public async Task<Result> ConnectAsync()
    {
        try
        {
            var options = await Options();
            var response = await MqttService.ConnectAsync(options);
            IsConnected = true;

            return Result.Success();
        }
        catch (Exception ex)
        {
            IsConnected = false;
            _logger.LogError($"Failed to connect to broker. {ex.Message}");

            return Result.Fail(message: "Failed to connect to broker.");
        }
    }

    public async Task DisconnectAsync()
    {
        IsConnected = false;
        await MqttService.DisconnectAsync();
    }

    /// <summary>
    /// Unsubscribes from all topics for device and removed device from device collection.
    /// </summary>
    /// <param name="device"></param>
    /// <returns></returns>
    public async Task UnsubscribeAsync(Device device)
    {
        foreach (var control in device.Controls)
        {
            var topic = await TopicService.RemoveTopic(Broker.Id, device, control);
            await MqttService.UnsubscribeAsync(topic);
        }

        Devices.Remove(device);
    }

    /// <summary>
    /// Unsubscribes from control topic and removes it from device controls collection.
    /// </summary>
    /// <param name="deviceId"></param>
    /// <param name="control"></param>
    /// <returns></returns>
    public async Task UnsubscribeAsync(string deviceId, string controlId)
    {
        var device = Devices.First(x => x.Id == deviceId);
        var control = device.Controls.First(x => x.Id == controlId);
        var topic = await TopicService.RemoveTopic(Broker.Id, device, control);
        
        if(IsConnected)
        {
            try 
            { 
                await MqttService.UnsubscribeAsync(topic); 
            }
            catch (Exception e)
            {
                _logger.LogError($"Failed to unsubscribe: {e}");
            }
        }

        device.Controls.Remove(control);
    }

    public async Task<bool> Resubscibe(Device device, Control newControl)
    {
        try
        {
            var control = device.Controls.First(x => x.Id == newControl.Id);

            var newTopic = newControl.GetTopic(device);
            var oldTopic = control.GetTopic(device);

            if(newTopic != oldTopic)
            {
                var value = await TopicService.LastMessageOnTopicAsync(device.BrokerId, device, control);

                if (!value.IsNullOrEmpty())
                {
                    await TopicService.UpdateMessageOnTopic(device.BrokerId, device, newControl, value);
                    await TopicService.RemoveTopic(Broker.Id, device, control);
                }

            }

            control.Update(newControl);


            if (!IsConnected)
                await ConnectAsync();

            await MqttService.UnsubscribeAsync(oldTopic);
            await MqttService.SubscribeAsync(newTopic, control.QualityOfService);
            
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError("." + ex.Message);
            return false;
        }
    }

    /// <summary>
    /// 
    /// Subscibes each control topic and adds it to device controls collection.
    /// </summary>
    /// <returns></returns>
    public async Task<int> SubscribeAsync(Device device, List<Control> controls)
    {
        Devices.Add(device);
        int failedSubscribtions = 0;

        foreach (var control in controls)
            if (!await SubscribeAsync(device, control))
                ++failedSubscribtions;

        return failedSubscribtions;
    }
    /// <summary>
    /// Subscibes to control topic and adds it to device controls collection.
    /// </summary>
    /// <returns></returns>
    public async Task<bool> SubscribeAsync(Device device, Control control)
    {
        try
        {
            if (TopicService.ConatinsTopic(Id, device, control))
                return true;

            var topic = await TopicService.AddTopic(Broker.Id, device, control);

            if (!MqttService.IsConnected)
                await ConnectAsync();

            device.Controls.Add(control);
            var result = await MqttService.SubscribeAsync(topic, control.QualityOfService);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError("." + ex.Message);
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
                    if (!isSync)
                        await UnsubscribeAsync(device.Id, existingControl!.Id);

                    var topic = await TopicService.AddTopic(Broker.Id, device, control);

                    if (!MqttService.IsConnected)
                        await ConnectAsync();

                    device.Controls.Add(control);
                    await MqttService.SubscribeAsync(topic);
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
                var topic = await TopicService.RemoveTopic(Broker.Id, device, control);
                await MqttService.UnsubscribeAsync(topic);
                device.Controls.Remove(control);
            }

        return failedSubscribtions;
    }

    public async ValueTask DisposeAsync()
    {
        foreach (var device in Devices)
            foreach (var control in device.Controls)
                await TopicService.RemoveTopic(Broker.Id, device, control);

        await MqttService.DisconnectAsync();
        MqttService.Dispose();
    }

    #region Privates

    private async Task<MqttClientOptions> Options()
    {
        var optionsBuilder = _factory.CreateClientOptionsBuilder()
            .WithClientId(Broker.ClientId)
            .WithWebSocketServer($"wss://{Broker.Server}:{Broker.Port}/mqtt")
            .WithCleanSession(false);

        var result = await BrokerService.GetBrokerCredentials(Broker.Id);

        //TODO: Notify about problem

        if (result.Succeeded && !string.IsNullOrEmpty(result.Data.Username) && !string.IsNullOrEmpty(result.Data.Password))
            optionsBuilder = optionsBuilder.WithCredentials(result.Data.Username, result.Data.Password);

        return optionsBuilder.Build();
    }

    private void InitializeCallbacks()
    {
        MqttService.ApplicationMessageReceivedAsync += async (e) =>
        {
            var topic = e.ApplicationMessage.Topic;
            var message = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
            await TopicService.UpdateMessageOnTopic(Broker.Id, topic, message);
            _logger.LogInformation($"Message received on topic: {topic}. Message: {message}");
            RerenderPage?.Invoke();
        };

        MqttService.DisconnectedAsync += async (e) =>
        {
            if (!IsConnected)
                return;

            _logger.LogWarning($"Client {Broker.Id} disconnected. Reconnecting...");
            RerenderPage?.Invoke();
            await MqttService.ReconnectAsync();
            RerenderPage?.Invoke();
            _logger.LogWarning($"Client {Broker.Id} reconnected.");
        };
    }

    #endregion
}
