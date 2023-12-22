using MQTTnet.Adapter;
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

    private readonly IMqttClient _mqttClient;
    private readonly IFetchBrokerService _brokerService;
    private readonly ILogger<Client> _logger;
    private readonly IStringLocalizer<Client> _localizer;
    private readonly IList<Device> _devices;
    private readonly IList<Control> _controls;
    private readonly Broker _broker;

    private Func<Task>? RerenderPageOnMessageReceived = null;

    public string Id => _broker.Id;
    public bool IsConnected { get; private set; }
    public ITopicService TopicService { get; private set; }

    public Client(ITopicService topic,
                  IMqttClient mqttClient,
                  IFetchBrokerService brokerService,
                  ILogger<Client> clientLogger,
                  IStringLocalizer<Client> localizer,
                  Broker broker)
    {
        _broker = broker;
        _mqttClient = mqttClient;
        _brokerService = brokerService;
        _localizer = localizer;
        TopicService = topic;
        _logger = clientLogger;

        InitializeCallbacks();

        _controls = new List<Control>();
        _devices = new List<Device>();
    }


    public void SetOnMessageReceivedEventHandler(Func<Task> refreshAction)
    {
        RerenderPageOnMessageReceived = refreshAction;
    }
    public void ClearOnMessageReceivedEventHandler() 
    {
        RerenderPageOnMessageReceived = null;
    }

    public Broker GetBroker() => _broker;
    public async Task UpdateBroker(Broker broker)
    {
        if(broker.EditedAt == _broker.EditedAt)
            return;

        var connected = _mqttClient.IsConnected;

        if (connected)
            await DisconnectAsync();

        _broker.Update(broker);

        if (connected)
            await ConnectAsync();
    }

    public async Task<IResult> AddControl(Control control)
    {
        try
        {
            var device = _devices.First(x => x.Id == control.DeviceId);
            _controls.Add(control);

            if (control.ShouldBeSubscribed() && _mqttClient.IsConnected)
            {
                var result = await _mqttClient.SubscribeAsync(control.GetTopic(device), control.QualityOfService);

                var status = result.Items.First().ResultCode;

                if (!ValidMqttResultCodes.Any(x => x == status))
                {
                    control.SubscribeStatus = ControlSubscribeStatus.FailedToSubscribe;
                    return Result.Warning(message: _localizer["Failed to subscribe to topic."]);
                }

                control.SubscribeStatus = ControlSubscribeStatus.Subscribed;
                return Result.Success();
            }

            control.SubscribeStatus = ControlSubscribeStatus.NotSubscribable;
            return Result.Success();
        }
        catch (ArgumentNullException e)
        {
            _logger.LogError("Failed to find device for {client}", _broker.Name);
            return Result.Fail(message: _localizer["Couldn't find device for control."]);
        }
        catch (Exception e)
        {
            _logger.LogError("Unknown error occured: {error}.", e.GetType());
            return Result.Fail(message: _localizer["Unknown error occured."]);
        }
    }
    public async Task<IResult> UpdateControl(Control control)
    {
        try
        {
            var device = _devices.First(x => x.Id == control.DeviceId);
            var currentControl = _controls.FirstOrDefault(x => x.Id == control.Id);

            if (currentControl is null)
                return await AddControl(control);

            if (control.ShouldBeSubscribed() && _mqttClient.IsConnected)
                await _mqttClient.UnsubscribeAsync(control.GetTopic(device));

            currentControl.Update(control);

            if (control.ShouldBeSubscribed() && _mqttClient.IsConnected)
            {
                var result = await _mqttClient.SubscribeAsync(control.GetTopic(device), control.QualityOfService);

                var status = result.Items.First().ResultCode;

                if (!ValidMqttResultCodes.Any(x => x == status))
                {
                    control.SubscribeStatus = ControlSubscribeStatus.FailedToSubscribe;
                    return Result.Warning();
                }

                control.SubscribeStatus = ControlSubscribeStatus.Subscribed;
                return Result.Success();
            }

            control.SubscribeStatus = ControlSubscribeStatus.NotSubscribable;
            return Result.Success();
        }
        catch (ArgumentNullException e)
        {
            _logger.LogError("Failed to find device for {client}", _broker.Name);
            return Result.Fail(message: _localizer["Couldn't find device for control."]);
        }
        catch (Exception e)
        {
            _logger.LogError("Unknown error occured: {error}.", e.GetType());
            return Result.Fail(message: _localizer["Unknown error occured."]);
        }
    }
    public async Task<IResult> RemoveControl(string controlId)
    {
        try
        {
            var control = _controls.First(x => x.Id == controlId);
            _controls.Remove(control);

            if (control.ShouldBeSubscribed() && _mqttClient.IsConnected)
            {
                var device = _devices.First(x => x.Id == control.DeviceId);
                await _mqttClient.UnsubscribeAsync(control.GetTopic(device));
            }

            return Result.Success();
        }
        catch (ArgumentNullException e)
        {
            _logger.LogWarning("Failed to find control for device: {error}.", e.GetType());
            return Result.Warning();
        }
        catch (MqttCommunicationException e)
        {
            _logger.LogWarning("Failed to disconnect control: {error}.", e.GetType());
            return Result.Warning();
        }
        catch (Exception e)
        {
            _logger.LogError("Unknown error occured: {error}.", e.GetType());
            return Result.Fail(message: _localizer["Unknown error occured."]);
        }
    }
    public IList<Control> GetControls(string deviceId) => _controls.Where(x => x.DeviceId == deviceId)
                                                                   .ToList();
    public IList<Control> GetControls() => _controls;

    public IList<Device> GetDevices() => _devices;
    public IResult AddDevice(Device device)
    {
        _devices.Add(device);

        device.SuccessfullControlsDownload = true;

        return Result.Success();
    }
    public async Task<IResult> AddDevice(Device device, IList<Control> controls)
    {
        _devices.Add(device);

        var status = Result.Success();

        foreach (var control in controls)
        {
            var result = await AddControl(control);

            if (status.OperationState ==  OperationState.Success && 
                result.OperationState == OperationState.Warning)
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
            var currentDevice = _devices.First(x => x.Id == device.Id);

            if (currentDevice.EditedAt == device.EditedAt)
                return Result.Success();

            var controls = _controls
                .Where(x => x.DeviceId == device.Id)
                .ToList();

            if(currentDevice.BaseDevicePath != device.BaseDevicePath)
            {
                foreach (var control in controls)
                    await RemoveControl(control.Id);

                currentDevice.Update(device);

                var status = Result.Success();

                foreach (var control in controls)
                {
                    var result = await AddControl(control);

                    if (status.OperationState ==  OperationState.Success && result.OperationState == OperationState.Warning)
                        status = Result.Warning();

                    if (result.OperationState == OperationState.Error)
                        status = Result.Fail();
                }

                if (status.OperationState == OperationState.Success)
                    device.SuccessfullControlsDownload = true;

                return status;
            }

            currentDevice.Update(device);

            return Result.Success();
        }
        catch (InvalidOperationException e)
        {
            _logger.LogError("Failed to find device: {error}.", e.GetType());
            return AddDevice(device);
        }
        catch (Exception e)
        {
            _logger.LogError("Unknown error occured: {error}.", e.GetType());
            return Result.Fail(message: _localizer["Unknown error occured."]);
        }
    }
    public async Task<IResult> UpdateDevice(Device device, IList<Control> controls)
    {
        try
        {
            var currentDevice = _devices.First(x => x.Id == device.Id);

            var oldControls = _controls.Where(x => x.DeviceId == device.Id)
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
        catch (ArgumentNullException e)
        {
            _logger.LogError("Failed to find device: {error}.", e.GetType());
            return AddDevice(device);
        }
        catch (Exception e)
        {
            _logger.LogError("Unknown error occured: {error}.", e.GetType());
            return Result.Fail(message: _localizer["Unknown error occured."]);
        }
    }
    public async Task<IResult> RemoveDevice(string deviceId)
    {
        try
        {
            var device = _devices.First(x => x.Id == deviceId);
            _devices.Remove(device);

            var controls = _controls.Where(x => x.DeviceId == deviceId)
                .ToList();

            foreach (var control in controls)
                await RemoveControl(control.Id);

            return Result.Success();
        }
        catch (ArgumentNullException e)
        {
            _logger.LogWarning("Failed to find device: {error}.", e.GetType());
            return Result.Warning();
        }
        catch (Exception e)
        {
            _logger.LogError("Unknown error occured: {error}.", e.GetType());
            return Result.Fail(message: _localizer["Unknown error occured."]);
        }
    }
    public bool HasDevice(string deviceId) => _devices.Any(x => x.Id == deviceId);

    public async Task<IResult> PublishAsync(string topic, string payload, MqttQualityOfServiceLevel quality)
    {
        try
        {
            if (!_mqttClient.IsConnected)
            {
                var connectionResult = await ConnectAsync();

                if (!connectionResult.Succeeded)
                    return connectionResult;
            }

            var mqttMessage = new MqttApplicationMessageBuilder()
                   .WithTopic(topic)
                   .WithPayload(Encoding.UTF8.GetBytes(payload))
                   .WithQualityOfServiceLevel(quality)
                   .Build();

            var mqttResult = await _mqttClient.PublishAsync(mqttMessage);

            if (mqttResult.ReasonCode != MqttClientPublishReasonCode.Success)
            {
                _logger.LogError("Failed to publish payload.");
                return Result.Fail(message: _localizer["Failed to publish data."]);
            }

            return Result.Success();
        }
        catch (Exception e)
        {
            _logger.LogError("Publish operation failed due to {error}.\n{message}", e.GetType(), e.Message);
            return Result.Fail(message: _localizer["Unknown error when publishing data."]);
        }
    }
    public async Task DisconnectAsync()
    {
        IsConnected = false;
        RerenderPageOnMessageReceived?.Invoke();
        await _mqttClient.DisconnectAsync();
    }
    public async Task<IResult> ConnectAsync()
    {
        try
        {
            if (!_broker.SSL)
            {
                _logger.LogError("You cannot connect to broker without secure connection.");
                return Result.Fail(message: _localizer["You cannot connect to broker without secure connection."]);
            }
            var optionsBuilder = new MqttClientOptionsBuilder()
                .WithClientId(_broker.ClientId)
                .WithWebSocketServer($"wss://{_broker.Server}:{_broker.Port}/mqtt")
                .WithCleanSession(true);

            var result = await _brokerService.GetBrokerCredentials(_broker.Id);

            if (!result.Succeeded)
            {
                _logger.LogError("Failed to fetch credentials.");
                return Result.Fail(message:_localizer["Failed to fetch credentials."]);
            }

            if (!string.IsNullOrEmpty(result.Data.Username) && !string.IsNullOrEmpty(result.Data.Password))
                optionsBuilder = optionsBuilder.WithCredentials(result.Data.Username, result.Data.Password);

            var response = await _mqttClient.ConnectAsync(optionsBuilder.Build());

            if(response.ResultCode == MqttClientConnectResultCode.NotAuthorized)
            {
                _logger.LogError("Failed to authorize.");
                return Result.Fail(message: _localizer["Failed to authorize."]);
            }
            else if (response.ResultCode != MqttClientConnectResultCode.Success)
            {
                _logger.LogError("Failed to connect to broker: {code}.", response.ResultCode);
                return Result.Fail(message: _localizer["Failed to connect to broker."]);
            }

            IsConnected = true;

            var status = await SubscribeToTopics();
            RerenderPageOnMessageReceived?.Invoke();

            return status;
        }
        catch (MqttConnectingFailedException cex)
        {
            IsConnected = false;
            _logger.LogError("Failed to authenticate to broker.", cex.Message);
            return Result.Fail(message: "Failed to authenticate to broker.");
        }
        catch (Exception e)
        {

            IsConnected = false;
            _logger.LogError("Failed to connect to broker.", e.Message);
            return Result.Fail(message: "Failed to connect to broker.");
        }
    }

    public async ValueTask DisposeAsync()
    {
        foreach (var control in _controls)
        {
            var device = _devices.First(x => x.Id == control.DeviceId);
            await TopicService.RemoveTopic(_broker.Id, device, control);
        }

        await _mqttClient.DisconnectAsync();
        _mqttClient.Dispose();
    }

    private async Task<IResult> SubscribeToTopics()
    {
        var status = Result.Success();

        foreach (var control in _controls)
        {
            if (!control.ShouldBeSubscribed())
            {
                control.SubscribeStatus = ControlSubscribeStatus.NotSubscribable;
                continue;
            }

            var device = _devices.First(x => x.Id == control.DeviceId);
            var subResult = await _mqttClient.SubscribeAsync(control.GetTopic(device), control.QualityOfService);

            if (!ValidMqttResultCodes.Any(x => x == subResult.Items.First().ResultCode))
            {
                control.SubscribeStatus = ControlSubscribeStatus.FailedToSubscribe;
                status = Result.Warning(message: _localizer["Failed to subscribe some topics."]);
            }
            else
            {
                control.SubscribeStatus = ControlSubscribeStatus.Subscribed;
            }
        }

        return status;
    }

    private void InitializeCallbacks()
    {
        _mqttClient.ApplicationMessageReceivedAsync += async (e) =>
        {
            var topic = e.ApplicationMessage.Topic;
            var message = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
            await TopicService.UpdateMessageOnTopic(_broker.Id, topic, message);
            _logger.LogInformation("Message received: {topic}\nMessage: {message}", topic, message);
            RerenderPageOnMessageReceived?.Invoke();
        };

        _mqttClient.DisconnectedAsync += async (e) =>
        {
            if (!IsConnected)
                return;

            _logger.LogWarning("Client disconnected. Reconnecting...", _broker.Id);
            await _mqttClient.ReconnectAsync();
            await SubscribeToTopics();
            RerenderPageOnMessageReceived?.Invoke();
            _logger.LogWarning("Client reconnected.", _broker.Id);
        };
    }
}
