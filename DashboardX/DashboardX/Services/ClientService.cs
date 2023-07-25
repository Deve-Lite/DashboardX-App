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

public class ClientService : IClientService
{
    private readonly ToastR _toastR;
    private readonly MqttFactory _factory;
    private readonly IBrokerService _brokerService;
    private readonly IDeviceService _deviceService;
    private readonly ITopicService _topicService;

    private IList<InitializedBroker> clients;

    private bool firstLoad;
    private bool updatedSuccessfully;
    public bool UpdatedSuccessfully { get; }

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

        updatedSuccessfully = true;
        firstLoad = true;
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

        if (!await UpdateBrokers(brokersResponse))
            return new List<InitializedBroker>();

        if (!await UpdateDevices(devicesResponse))
            return new List<InitializedBroker>();

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


        if (!await UpdateBroker(brokerResponse))
            return new InitializedBroker();

        if (!await UpdateDevices(devicesResponse))
            return new InitializedBroker();


        return clients!.FirstOrDefault(x => x.Id == id)!;
    }

    #region Privates
    private async Task Initialize()
    {
        var brokerResponse = await _brokerService.GetBrokers();
        var deviceResponse = await _deviceService.GetDevices();

        if (!await InitalizeBrokers(brokerResponse))
            return;

        if (!await InitializeDevices(deviceResponse))
            return;
    }
    private async Task<bool> InitalizeBrokers(Response<List<Broker>> response)
    {
        if (!await SuccessfullResponse(response))
            return false;

        var brokers = response.Data;

        foreach (var broker in brokers)
            await ConnectBroker(broker);

        return true;
    }
    private async Task<bool> InitializeDevices(Response<List<Device>> response)
    {
        if (!await SuccessfullResponse(response))
            return false;

        var devices = response.Data;

        foreach (var device in devices)
        {
            var broker = clients.FirstOrDefault(x => x.Id == device.BrokerId)!;
            await SubscribeDeviceControls(broker, device);
        }

        return true;
    }
    private async Task SubscribeDeviceControls(InitializedBroker broker, Device device)
    {
        foreach (var control in device.GetControls())
        {
            var topic = control.GetTopic(device);
            await broker.Client.SubscribeAsync(topic);
        }

        broker.Devices[device.DeviceId] = device;
    }

    private async Task<bool> UpdateBrokers(Response<List<Broker>> response)
    {
        if (!await SuccessfullResponse(response))
            return false;

        var brokers = response.Data;

        var updatedBrokers = new HashSet<string>();

        foreach (var broker in brokers)
        {
            var existingClient = clients.FirstOrDefault(x => x.Id == broker.BrokerId);

            if (existingClient != null)
            {
                updatedBrokers.Add(existingClient.Id);

                if (existingClient.Broker.EditedAt != broker.EditedAt)
                {
                    await existingClient.Client.DisconnectAsync();
                    clients.Remove(existingClient);
                    await ConnectBroker(broker);
                }
            }
            else
                await ConnectBroker(broker);
        }

        var clientsToRemove = clients.Where(client => !updatedBrokers.Contains(client.Id)).ToList();

        foreach (var client in clientsToRemove)
        {
            await client.Client.DisconnectAsync();
            clients.Remove(client);
        }

        return true;
    }
    private async Task<bool> UpdateBroker(Response<Broker> response)
    {
        if (!await SuccessfullResponse(response))
            return false;

        var broker = response.Data;

        var existingClient = clients.FirstOrDefault(x => x.Id == broker.BrokerId);

        if (existingClient != null && existingClient.Broker.EditedAt != broker.EditedAt)
        {
            await existingClient.Client.DisconnectAsync();
            clients.Remove(existingClient);
            await ConnectBroker(broker);
        }
        else
            await ConnectBroker(broker);

        return true;
    }

    private async Task<bool> UpdateDevices(Response<List<Device>> response)
    {
        if (!await SuccessfullResponse(response))
            return false;

        var devices = response.Data;

        var updatedDevices = new HashSet<string>();

        foreach (var newDevice in devices)
        {
            //Broker must exists because broker is created before devices
            var broker = clients.FirstOrDefault(x => x.Id == newDevice.BrokerId)!;

            if (broker.Devices.ContainsKey(newDevice.DeviceId))
            {
                updatedDevices.Add(newDevice.DeviceId);

                var currentDevice = broker.Devices[newDevice.DeviceId];

                if (currentDevice.EditedAt != newDevice.EditedAt)
                {
                    await UnsubscribeDeviceControls(broker, currentDevice);
                    await SubscribeDeviceControls(broker, newDevice);
                }
            }
            else
            {
                broker.Devices[newDevice.DeviceId] = newDevice;
                await SubscribeDeviceControls(broker, newDevice);
            }
        }

        foreach (var client in clients)
            foreach (var device in client.Devices)
                if(!updatedDevices.Contains(device.Key))
                    await UnsubscribeDeviceControls(client, device.Value);
            
        return true;
    }

    private async Task UnsubscribeDeviceControls(InitializedBroker broker, Device device)
    {
        foreach (var control in device.GetControls())
        {
            var topic = control.GetTopic(device);
            await broker.Client.UnsubscribeAsync(topic);
        }

        broker.Devices.Remove(device.DeviceId);
    }

    #endregion

    #region Conditions

    public async Task<bool> SuccessfullResponse<T>(Response<T> response) where T : class, new()
    {
        if (response.Success)
        {
            await _toastR.Error("Failed to load brokers and devices.");
            updatedSuccessfully = false;
            return false;
        }
        updatedSuccessfully = true;
        return true;
    }

    #endregion

    #region MQTT Actions

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

            OnMessageReceived.Invoke();

            return Task.CompletedTask;
        };
        client.DisconnectedAsync += Disconnect;
    }
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
