using Blazored.LocalStorage;
using Core.Interfaces;
using Infrastructure.Services;
using MQTTnet;
using MQTTnet.Client;
using Shared.Models.Brokers;
using Shared.Models.Controls;
using Shared.Models.Devices;
using System.Text;

namespace Presentation.Models;

public class Client : IAsyncDisposable
{

    private readonly ILogger<Client> _logger;
    private readonly MqttFactory _factory;
    public readonly ITopicService TopicService;

    public string Id => Broker.Id;
    public bool IsConnected => Service.IsConnected;

    public Broker Broker { get; private set; } = new();
    public IMqttClient Service { get; private set; }
    public List<Device> Devices { get; private set; } = new();

    public Func<Task> RerenderPage { get; set; }

    public Client(ILocalStorageService storage, ILogger<Client> clientLogger, MqttFactory factory, Broker broker)
    {
        Broker = broker;
        Service = factory.CreateMqttClient();

        TopicService = new TopicService(storage);
        _factory = factory;
        _logger = clientLogger;

        InitializeCallbacks();
        RerenderPage += ()=> { return Task.CompletedTask; };
    }

    public async Task UpdateBroker(Broker broker)
    {
        var connected = Service.IsConnected;

        if (connected)
            await DisconnectAsync();

        Broker = broker;

        if (connected)
            await ConnectAsync();
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
            var topic = await TopicService.RemoveTopic(Broker.Id, device, control);
            await Service.UnsubscribeAsync(topic);
        }

        Devices.Remove(device);
    }
    public async Task UnsubscribeAsync(string deviceId, Control control)
    {
        var device = Devices.First(x => x.Id == deviceId);
        var topic = await TopicService.RemoveTopic(Broker.Id, device, control);
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
            if (TopicService.ConatinsTopic(Id, device, control))
                return true;

            var topic = await TopicService.AddTopic(Broker.Id, device, control);

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

                    var topic = await TopicService.AddTopic(Broker.Id, device, control);

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
                var topic = await TopicService.RemoveTopic(Broker.Id, device, control);
                await Service.UnsubscribeAsync(topic);
                device.Controls.Remove(control);
            }

        return failedSubscribtions;
    }

    public async ValueTask DisposeAsync()
    {
        foreach (var device in Devices)
            foreach (var control in device.Controls)
                await TopicService.RemoveTopic(Broker.Id, device, control);

        await Service.DisconnectAsync();
        Service.Dispose();
    }

    #region Privates

    private MqttClientOptions Options()
    {
        var optionsBuilder = _factory.CreateClientOptionsBuilder()
            .WithClientId(Broker.ClientId)
            .WithWebSocketServer($"wss://{Broker.Server}:{Broker.Port}/mqtt")
            .WithCleanSession(false);

        if (!string.IsNullOrEmpty(Broker.Username) && !string.IsNullOrEmpty(Broker.Password))
            optionsBuilder = optionsBuilder.WithCredentials(Broker.Username, Broker.Password);

        return optionsBuilder.Build();
    }

    private void InitializeCallbacks()
    {
        Service.ApplicationMessageReceivedAsync += async(e) =>
        {
            var topic = e.ApplicationMessage.Topic;
            var message = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
            await TopicService.UpdateMessageOnTopic(Broker.Id, topic, message);
            _logger.LogInformation($"Message received on topic: {topic}. Message: {message}");
            RerenderPage?.Invoke();
        };

        // TODO: Include case of manual disconnection
        Service.DisconnectedAsync += async(e) =>
        {
            _logger.LogWarning($"Client {Broker.Id} disconnected. Reconnecting...");
            RerenderPage?.Invoke();
            await Service.ReconnectAsync();
            RerenderPage?.Invoke();
            _logger.LogWarning($"Client {Broker.Id} reconnected.");
        };
    }

    #endregion
}
