using DashboardX.Brokers;
using DashboardX.Devices;
using DashboardX.Helpers;
using DashboardX.Services.Interfaces;
using DashboardXModels;
using DashboardXModels.Brokers;
using DashboardXModels.Controls;
using DashboardXModels.Devices;
using MQTTnet;
using MQTTnet.Client;
using System.Text;

namespace DashboardX.Services;

public class ClientService
{
    private readonly ToastR _toastR;
    private readonly MqttFactory _factory;
    private readonly IBrokerService _brokerService;
    private readonly IDeviceService _deviceService;
    private readonly ITopicService _topicService;

    private IList<InitializedBroker> clients;
    private bool firstLoad = true;

    public bool UpdatedSuccessfully { get; private set; }

    public Action OnMessageReceived { get; set; }

    public ClientService(ToastR toastR, 
                         MqttFactory factory,
                         IBrokerService brokerService, 
                         IDeviceService deviceService,
                         ITopicService topicService)
    {
        _topicService = topicService;
        _toastR = toastR;
        _deviceService = deviceService;
        _brokerService = brokerService;
        _factory = factory;

        clients = new List<InitializedBroker>();

        OnMessageReceived = () => { };
    }

    public async Task<IList<InitializedBroker>> GetInitializedBrokers()
    {
        if (firstLoad)
        {
            await Initialize();
            firstLoad = false;
            return clients;
        }

        var brokersResponse = await _brokerService.GetBrokers();
        var devicesResponse = await _deviceService.GetDevices();

        // TODO : Update brokers

        // TODO : Update devices

        return clients;
    }
    public async Task<InitializedBroker> GetInitializedBroker(string id)
    {
        if (firstLoad)
        {
            await Initialize();
            firstLoad = false;
            return clients.FirstOrDefault(x => x.Id == id)!;
        }

        var brokerResponse = await _brokerService.GetBroker(id);
        var devicesResponse = await _deviceService.GetDevices(id);

        var currentBroker = clients.FirstOrDefault(x => x.Id == id)!;

        // TODO : Update broker 

        // TODO : Update devices


        return clients!.FirstOrDefault(x => x.Id == id)!;
    }
    private async Task Initialize()
    {
        var brokerResponse = await _brokerService.GetBrokers();
        var deviceResponse = await _deviceService.GetDevices();

        if(!await InitalizeBrokers(brokerResponse))
            return;

        if (!await InitializeDevices(deviceResponse))
            return;
    }

    #region Initialization

    private async Task<bool> InitalizeBrokers(Response<List<Broker>> response)
    {
        if (response.Success)
        {
            await _toastR.Error("Failed to load brokers and devices.");
            UpdatedSuccessfully = false;
            return false;
        }

        var brokers = response.Data;

        foreach (var broker in brokers)
            await ConnectBroker(broker);

        return true;
    }
    private async Task<bool> InitializeDevices(Response<List<Device>> response) 
    {
        if (!response.Success)
        {
            await _toastR.Error("Failed to load devices.");
            UpdatedSuccessfully = false;
            return false;
        }

        var devices = response.Data;

        foreach (var device in devices)
        {
            var broker = clients!.FirstOrDefault(x => x.Id == device.BrokerId)!;

            InitializeControls(broker, device);
            
            broker.Devices[device.DeviceId] = device;
        }

        return true;
    }
    public void InitializeControls(InitializedBroker broker, Device device)
    {
        foreach(var control in device.GetControls())
        {
            var topic = control.GetTopic(device);
            broker.Client.SubscribeAsync(topic);
        }
    }

    private async Task ConnectBroker(Broker broker)
    { 
        var client = await ConnectMqttClient(broker);
        var initializedBroker = new InitializedBroker(broker, client);

        clients.Add(initializedBroker);
    }
    private async Task<IMqttClient> ConnectMqttClient(Broker broker)
    {
        var client = _factory.CreateMqttClient();

        var options = _factory.CreateClientOptionsBuilder()
            .WithClientId(broker.ClientId)
            .WithWebSocketServer($"wss://{broker.Server}:{broker.Port}")
            .WithCredentials(broker.Username, broker.Password)
            .Build();

        await client.ConnectAsync(options);

        InitializeCallbacks(client, broker);

        return client;
    }
    private void InitializeCallbacks(IMqttClient client, Broker broker)
    {
        client.ApplicationMessageReceivedAsync += (e) =>
        {
            var topic = e.ApplicationMessage.Topic;
            var message = Encoding.UTF8.GetString(e.ApplicationMessage.PayloadSegment);

            _topicService.UpdateTopic(broker.BrokerId, topic, message);

            return Task.CompletedTask;
        };
        client.DisconnectedAsync += Disconnect;
    }

    #endregion

    #region Events

    private Task Disconnect(MqttClientDisconnectedEventArgs e)
    {
        return Task.CompletedTask;
    }

    #endregion

    #region Consts

    public const string BrokersFail = "Failed to load brokers and devices.";
    public const string BrokerFail = "Failed to load broker and connected devices.";
    public const string DevicesFail = "Failed to load devices.";

    #endregion
}
