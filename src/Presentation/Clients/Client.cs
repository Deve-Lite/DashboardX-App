using Core.App.Interfaces;
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
    private readonly ISnackbar _snackbar;
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
                  ISnackbar snackbar,
                  Broker broker)
    {
        _broker = broker;
        _mqttClient = mqttClient;
        _brokerService = brokerService;
        TopicService = topic;
        _snackbar = snackbar;
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
            var control = _controls.First(x => x.Id == controlId);
            _controls.Remove(control);

            if (control.ShouldBeSubscribed() && _mqttClient.IsConnected)
            {
                var device = _devices.First(x => x.Id == control.DeviceId);
                await _mqttClient.UnsubscribeAsync(control.GetTopic(device));
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
    public IList<Control> GetControls(string deviceId) => _controls.Where(x => x.DeviceId == deviceId).ToList();
    public IList<Control> GetControls() => _controls;
    public IList<Device> GetDevices() => _devices;
    public IResult AddDevice(Device device)
    {
        _devices.Add(device);

        device.SuccessfullControlsDownload = true;

        return Result.Success();
    }
    public async Task<IResult> AddDevices(Device device, List<Control> controls)
    {
        _devices.Add(device);

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
            var currentDevice = _devices.First(x => x.Id == device.Id);

            if (currentDevice.EditedAt == device.EditedAt)
                return Result.Success();

            var controls = _controls.Where(x => x.DeviceId == device.Id)
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
        catch (ArgumentNullException)
        {
            return AddDevice(device);
        }
        catch (Exception ex)
        {
            return Result.Fail();
        }
    }
    public async Task<IResult> UpdateDevice(Device device, List<Control> controls)
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
            var device = _devices.First(x => x.Id == deviceId);
            _devices.Remove(device);

            var controls = _controls.Where(x => x.DeviceId == deviceId)
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
    public bool HasDevice(string deviceId) => _devices.Any(x => x.Id == deviceId);
   
    public async Task DisconnectAsync()
    {
        IsConnected = false;
        RerenderPageOnMessageReceived?.Invoke();
        await _mqttClient.DisconnectAsync();
    }
    public async Task<IResult> PublishAsync(string topic, string payload, MqttQualityOfServiceLevel quality)
    {
        try
        {
            if (!_mqttClient.IsConnected)
                await ConnectAsync();

            var mqttMessage = new MqttApplicationMessageBuilder()
                   .WithTopic(topic)
                   .WithPayload(Encoding.UTF8.GetBytes(payload))
                   .WithQualityOfServiceLevel(quality)
                   .Build();

            var mqttResult = await _mqttClient.PublishAsync(mqttMessage);

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
            if (!_broker.SSL)
                return Result.Fail();

            var optionsBuilder = new MqttClientOptionsBuilder()
                .WithClientId(_broker.ClientId)
                .WithWebSocketServer($"wss://{_broker.Server}:{_broker.Port}/mqtt")
                .WithCleanSession(true);

            var result = await _brokerService.GetBrokerCredentials(_broker.Id);

            if (result.Succeeded && !string.IsNullOrEmpty(result.Data.Username) && !string.IsNullOrEmpty(result.Data.Password))
                optionsBuilder = optionsBuilder.WithCredentials(result.Data.Username, result.Data.Password);

            var response = await _mqttClient.ConnectAsync(optionsBuilder.Build());

            var status = Result.Success();

            foreach (var control in _controls)
            {
                if (!control.ShouldBeSubscribed())
                    continue;

                var device = _devices.First(x => x.Id == control.DeviceId);
                var subResult = await _mqttClient.SubscribeAsync(control.GetTopic(device), control.QualityOfService);

                if(!ValidMqttResultCodes.Any(x => x == subResult.Items.First().ResultCode))
                    status = Result.Warning();
            }

            IsConnected = true;
            RerenderPageOnMessageReceived?.Invoke();

            return status;
        }
        catch (MqttConnectingFailedException cex)
        {
            _snackbar.Add("Failed when authenticating to broker.", Severity.Error);
            _logger.LogError("Failed to authenticate to broker.", cex.Message);
            IsConnected = false;
            return Result.Fail(message: "Failed to authenticate to broker.");
        }
        catch (Exception e)
        {
            _snackbar.Add("Failed when connecting to broker.", Severity.Error);
            _logger.LogError("Failed to connect to broker.", e.Message);
            IsConnected = false;
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

    private void InitializeCallbacks()
    {
        _mqttClient.ApplicationMessageReceivedAsync += async (e) =>
        {
            var topic = e.ApplicationMessage.Topic;
            var message = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
            await TopicService.UpdateMessageOnTopic(_broker.Id, topic, message);
            _logger.LogInformation("Message received. {topic} {message}", topic, message);
            RerenderPageOnMessageReceived?.Invoke();
        };

        _mqttClient.DisconnectedAsync += async (e) =>
        {
            if (!IsConnected)
                return;

            _logger.LogWarning("Client disconnected. Reconnecting...", _broker.Id);
            RerenderPageOnMessageReceived?.Invoke();
            await _mqttClient.ReconnectAsync();
            RerenderPageOnMessageReceived?.Invoke();
            _logger.LogWarning("Client reconnected.", _broker.Id);
        };
    }
}
