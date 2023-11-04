using Common.Controls.Models;
using Common.Devices.Models;
using Microsoft.IdentityModel.Tokens;
using MQTTnet.Client;
using MQTTnet.Protocol;
using MQTTnet.Server;
using System.Text;

namespace Presentation.Clients;

public class Client : IClient, IAsyncDisposable
{
    private static readonly MqttClientSubscribeResultCode[] ValidMqttResultCodes = new[]
    {
        MqttClientSubscribeResultCode.GrantedQoS0,
        MqttClientSubscribeResultCode.GrantedQoS1,
        MqttClientSubscribeResultCode.GrantedQoS2
    };

    private readonly ILogger<Client> _logger;
    private readonly ITopicService topicService;
    public ITopicService TopicService => topicService;
    public readonly IMqttClient MqttClient;
    public readonly IBrokerService BrokerService;
    public string Id => Broker.Id;
    public bool IsConnected { get; set; }

    public Broker Broker { get; private set; } = new();
    public List<Device> Devices { get; private set; } = new();
    public List<Control> Controls { get; private set; } = new();
    public Func<Task> RerenderPage { get; set; }

    public Client(ITopicService topic,
                  IMqttClient mqttClient,
                  IBrokerService brokerService,
                  ILogger<Client> clientLogger,
                  Broker broker)
    {
        Broker = broker;
        MqttClient = mqttClient;
        BrokerService = brokerService;
        topicService = topic;

        _logger = clientLogger;

        InitializeCallbacks();
        RerenderPage += () =>
        {
            return Task.CompletedTask;
        };
    }

    public async Task UpdateBroker(Broker broker)
    {
        var connected = MqttClient.IsConnected;

        if (connected)
            await DisconnectAsync();

        Broker = broker;

        if (connected)
            await ConnectAsync();
    }

    public async Task<IResult> AddControl(Control control)
    {
        try
        {
            var device = Devices.First(x => x.Id == control.DeviceId);
            Controls.Add(control);

            if (control.ShouldBeSubscribed() && MqttClient.IsConnected)
            {
                var result = await MqttClient.SubscribeAsync(control.GetTopic(device), control.QualityOfService);

                var status = result.Items.First().ResultCode;

                if (!ValidMqttResultCodes.Any(x => x == status))
                    return Result.Warning();

                control.IsSubscribed = true;
            }

            return Result.Success();
        }
        catch (ArgumentNullException)
        {
            return Result.Fail();
        }
        catch
        {
            return Result.Fail();
        }
    }

    public async Task<IResult> UpdateControl(Control control)
    {
        try
        {
            var device = Devices.First(x => x.Id == control.DeviceId);
            var currentControl = Controls.First(x => x.Id == control.Id);

            if (control.ShouldBeSubscribed() && MqttClient.IsConnected)
                await MqttClient.UnsubscribeAsync(control.GetTopic(device));

            currentControl.Update(control);

            if (control.ShouldBeSubscribed() && MqttClient.IsConnected)
            {
                var result = await MqttClient.SubscribeAsync(control.GetTopic(device), control.QualityOfService);

                var status = result.Items.First().ResultCode;

                if (!ValidMqttResultCodes.Any(x => x == status))
                    return Result.Warning();

                control.IsSubscribed = true;
            }

            return Result.Success();
        }
        catch (ArgumentNullException)
        {
            return Result.Fail();
        }
        catch
        {
            return Result.Fail();
        }
    }

    public async Task<IResult> RemoveControl(string controlId)
    {
        try
        {
            var control = Controls.First(x => x.Id == controlId);
            var device = Devices.First(x => x.Id == control.DeviceId);

            if (control.ShouldBeSubscribed() || MqttClient.IsConnected)
                await MqttClient.UnsubscribeAsync(control.GetTopic(device));

            return Result.Success();
        }
        catch (ArgumentNullException)
        {
            return Result.Warning();
        }
        catch
        {
            return Result.Fail();
        }
    }

    public IResult AddDevice(Device device)
    {
        Devices.Add(device);

        return Result.Success();
    }

    public async Task<IResult> AddDevice(Device device, List<Control> controls)
    {
        Devices.Add(device);

        var status = Result.Success();

        foreach (var control in controls)
        {
            var result = await AddControl(control);

            if (status.OperationState ==  OperationState.Success && result.OperationState == OperationState.Warning)
                status = Result.Warning();

            if (result.OperationState == OperationState.Error)
                status = Result.Fail();
        }

        return status;
    }

    public async Task<IResult> UpdateDevice(Device device)
    {
        try
        {
            var currentDevice = Devices.First(x => x.Id == device.Id);

            if (currentDevice.EditedAt == device.EditedAt)
                return Result.Success();

            var controls = Controls.Where(x => x.DeviceId == device.Id)
                .ToList();

            if(currentDevice.BaseDevicePath != device.BaseDevicePath)
            {
                foreach (var control in controls)
                    await RemoveControl(control.Id);
                
                Devices.Remove(currentDevice);

                return await AddDevice(device, controls);
            }

            currentDevice.Update(device);

            return Result.Success();
        }
        catch (ArgumentNullException)
        {
            return Result.Warning();
        }
        catch
        {
            return Result.Fail();
        }
    }

    public async Task<IResult> RemoveDevice(string deviceId)
    {
        try
        {
            var device = Devices.First(x => x.Id == deviceId);
            Devices.Remove(device);

            var controls = Controls.Where(x => x.DeviceId == deviceId)
                .ToList();

            foreach (var control in controls)
                await RemoveControl(control.Id);

            return Result.Success();
        }
        catch (ArgumentNullException)
        {
            return Result.Warning();
        }
        catch
        {
            return Result.Fail();
        }
    }



    public async Task<MqttClientPublishResult> PublishAsync(string topic, string payload, MqttQualityOfServiceLevel quality)
    {
        //TODO Change to Result

        try
        {
            if (!MqttClient.IsConnected)
                await ConnectAsync();

            var mqttMessage = new MqttApplicationMessageBuilder()
                   .WithTopic(topic)
                   .WithPayload(Encoding.UTF8.GetBytes(payload))
                   .WithQualityOfServiceLevel(quality)
                   .Build();

            return await MqttClient.PublishAsync(mqttMessage);
        }
        catch (Exception e)
        {
            _logger.LogError("Failed to send request", e.Message);

            return new MqttClientPublishResult();
        }
    }

    public async Task<Result> ConnectAsync()
    {
        try
        {
            var options = await Options();
            var response = await MqttClient.ConnectAsync(options);
            IsConnected = true;

            return Result.Success();
        }
        catch (Exception e)
        {
            IsConnected = false;
            _logger.LogError("Failed to connect to broker.", e.Message);

            return Result.Fail(message: "Failed to connect to broker.");
        }
    }

    public async Task DisconnectAsync()
    {
        IsConnected = false;
        await MqttClient.DisconnectAsync();
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
            var topic = await topicService.RemoveTopic(Broker.Id, device, control);
            await MqttClient.UnsubscribeAsync(topic);
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
        var topic = await topicService.RemoveTopic(Broker.Id, device, control);

        if (IsConnected)
        {
            try
            {
                await MqttClient.UnsubscribeAsync(topic);
            }
            catch (Exception e)
            {
                _logger.LogError("Failed to unsubscribe to topic.", e.Message);
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

            if (newTopic != oldTopic)
            {
                var value = await topicService.LastMessageOnTopicAsync(device.BrokerId, device, control);

                if (!value.IsNullOrEmpty())
                {
                    await topicService.UpdateMessageOnTopic(device.BrokerId, device, newControl, value);
                    await topicService.RemoveTopic(Broker.Id, device, control);
                }

            }

            control.Update(newControl);


            if (!IsConnected)
                await ConnectAsync();

            await MqttClient.UnsubscribeAsync(oldTopic);
            await MqttClient.SubscribeAsync(newTopic, control.QualityOfService);

            return true;
        }
        catch (Exception e)
        {
            _logger.LogError("Failed to resubscribe to topic.", e.Message);
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
            if (topicService.ConatinsTopic(Id, device, control))
                return true;

            var topic = await topicService.AddTopic(Broker.Id, device, control);

            if (!MqttClient.IsConnected)
                await ConnectAsync();

            device.Controls.Add(control);
            var result = await MqttClient.SubscribeAsync(topic, control.QualityOfService);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to subscribe to topic.", ex.Message);
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

                    var topic = await topicService.AddTopic(Broker.Id, device, control);

                    if (!MqttClient.IsConnected)
                        await ConnectAsync();

                    device.Controls.Add(control);
                    await MqttClient.SubscribeAsync(topic);
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
                var topic = await topicService.RemoveTopic(Broker.Id, device, control);
                await MqttClient.UnsubscribeAsync(topic);
                device.Controls.Remove(control);
            }

        return failedSubscribtions;
    }

    public async ValueTask DisposeAsync()
    {
        foreach (var device in Devices)
            foreach (var control in device.Controls)
                await topicService.RemoveTopic(Broker.Id, device, control);

        await MqttClient.DisconnectAsync();
        MqttClient.Dispose();
    }

    #region Privates

    private async Task<MqttClientOptions> Options()
    {
        var optionsBuilder = new MqttClientOptionsBuilder()
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
        MqttClient.ApplicationMessageReceivedAsync += async (e) =>
        {
            var topic = e.ApplicationMessage.Topic;
            var message = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
            await topicService.UpdateMessageOnTopic(Broker.Id, topic, message);
            _logger.LogInformation("Message received. {topic} {message}", topic, message);
            RerenderPage?.Invoke();
        };

        MqttClient.DisconnectedAsync += async (e) =>
        {
            if (!IsConnected)
                return;

            _logger.LogWarning("Client disconnected. Reconnecting...", Broker.Id);
            RerenderPage?.Invoke();
            await MqttClient.ReconnectAsync();
            RerenderPage?.Invoke();
            _logger.LogWarning("Client reconnected.", Broker.Id);
        };
    }

    #endregion
}
