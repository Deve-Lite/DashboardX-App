using Common.Devices.Models;
using MQTTnet.Client;
using MQTTnet.Exceptions;
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

    private readonly IMqttClient MqttClient;
    private readonly IFetchBrokerService BrokerService;
    private readonly ILogger<Client> _logger;

    public string Id => Broker.Id;
    public bool IsConnected => MqttClient.IsConnected;

    public Broker Broker { get; private set; }
    public ITopicService TopicService { get; private set; }
    public IList<Device> Devices { get; private set; }
    public IList<Control> Controls { get; private set; }
    public Func<Task> RerenderPage { get; set; }

    public Client(ITopicService topic,
                  IMqttClient mqttClient,
                  IFetchBrokerService brokerService,
                  ILogger<Client> clientLogger,
                  Broker broker)
    {
        Broker = broker;
        MqttClient = mqttClient;
        BrokerService = brokerService;
        TopicService = topic;

        _logger = clientLogger;

        InitializeCallbacks();
        RerenderPage += () =>
        {
            return Task.CompletedTask;
        };

        Controls = new List<Control>();
        Devices = new List<Device>();
    }

    public async Task UpdateBroker(Broker broker)
    {
        if(broker.EditedAt == Broker.EditedAt)
            return;

        var connected = MqttClient.IsConnected;

        if (connected)
            await DisconnectAsync();

        Broker.Update(broker);

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
            var currentControl = Controls.FirstOrDefault(x => x.Id == control.Id);

            if (currentControl is null)
                return await AddControl(control);

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
            Controls.Remove(control);

            if (control.ShouldBeSubscribed() && MqttClient.IsConnected)
            {
                var device = Devices.First(x => x.Id == control.DeviceId);
                await MqttClient.UnsubscribeAsync(control.GetTopic(device));
            }

            return Result.Success();
        }
        catch (ArgumentNullException)
        {
            return Result.Warning();
        }
        catch (MqttCommunicationException)
        {
            //Failed to unsubscribe
            return Result.Warning();
        }
        catch
        {
            return Result.Fail();
        }
    }
    public IList<Control> GetControls(string deviceId) => Controls.Where(x => x.DeviceId == deviceId).ToList();

    public IResult AddDevice(Device device)
    {
        Devices.Add(device);

        device.SuccessfullControlsDownload = true;

        return Result.Success();
    }
    public async Task<IResult> AddDevices(Device device, List<Control> controls)
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

        if(status.OperationState == OperationState.Success)
            device.SuccessfullControlsDownload = true;

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

                return await AddDevices(device, controls);
            }

            currentDevice.Update(device);

            return Result.Success();
        }
        catch (ArgumentNullException)
        {
            return AddDevice(device);
        }
        catch
        {
            return Result.Fail();
        }
    }
    public async Task<IResult> UpdateDevice(Device device, List<Control> controls)
    {
        try
        {
            var currentDevice = Devices.First(x => x.Id == device.Id);

            var oldControls = Controls.Where(x => x.DeviceId == device.Id)
                .ToList();

            foreach (var control in oldControls)
                await RemoveControl(control.Id);

            currentDevice.Update(device);

            var status = Result.Success();

            foreach (var control in controls)
            {
                var result = await AddControl(control);

                if (result.OperationState == OperationState.Error || result.OperationState == OperationState.Warning)
                    return Result.Warning();
            }

            return status;
        }
        catch (ArgumentNullException)
        {
            return AddDevice(device);
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
    public bool HasDevice(string deviceId) => Devices.Any(x => x.Id == deviceId);
   
    public async Task DisconnectAsync()
    {
        await MqttClient.DisconnectAsync();
    }
    public async Task<IResult> PublishAsync(string topic, string payload, MqttQualityOfServiceLevel quality)
    {
        try
        {
            if (!MqttClient.IsConnected)
                await ConnectAsync();

            var mqttMessage = new MqttApplicationMessageBuilder()
                   .WithTopic(topic)
                   .WithPayload(Encoding.UTF8.GetBytes(payload))
                   .WithQualityOfServiceLevel(quality)
                   .Build();

            var mqttResult = await MqttClient.PublishAsync(mqttMessage);

            if (mqttResult.ReasonCode != MqttClientPublishReasonCode.Success)
                return Result.Fail();

            return Result.Success();
        }
        catch (Exception e)
        {
            _logger.LogError("Failed to send request", e.Message);

            return Result.Fail();
        }
    }
    public async Task<IResult> ConnectAsync()
    {
        //TODO: Refactor
        try
        {
            if (!Broker.SSL)
                return Result.Fail();

            var optionsBuilder = new MqttClientOptionsBuilder()
                .WithClientId(Broker.ClientId)
                .WithWebSocketServer($"wss://{Broker.Server}:{Broker.Port}/mqtt")
                .WithCleanSession(true);

            var result = await BrokerService.GetBrokerCredentials(Broker.Id);

            if (result.Succeeded && !string.IsNullOrEmpty(result.Data.Username) && !string.IsNullOrEmpty(result.Data.Password))
                optionsBuilder = optionsBuilder.WithCredentials(result.Data.Username, result.Data.Password);

            var response = await MqttClient.ConnectAsync(optionsBuilder.Build());

            var status = Result.Success();

            //Think what happens on += InitializeCallbacks();

            foreach (var control in Controls)
            {
                if (!control.ShouldBeSubscribed())
                    continue;

                var device = Devices.First(x => x.Id == control.DeviceId);
                var subResult = await MqttClient.SubscribeAsync(control.GetTopic(device), control.QualityOfService);

                if(!ValidMqttResultCodes.Any(x => x == subResult.Items.First().ResultCode))
                    status = Result.Warning();
            }

            return status;
        }
        catch (Exception e)
        {
            _logger.LogError("Failed to connect to broker.", e.Message);

            return Result.Fail(message: "Failed to connect to broker.");
        }
    }

    public async ValueTask DisposeAsync()
    {
        foreach (var control in Controls)
        {
            var device = Devices.First(x => x.Id == control.DeviceId);
            await TopicService.RemoveTopic(Broker.Id, device, control);
        }

        await MqttClient.DisconnectAsync();
        MqttClient.Dispose();
    }

    private void InitializeCallbacks()
    {
        MqttClient.ApplicationMessageReceivedAsync += async (e) =>
        {
            var topic = e.ApplicationMessage.Topic;
            var message = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
            await TopicService.UpdateMessageOnTopic(Broker.Id, topic, message);
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
}
