using Common.Controls.Models;
using MQTTnet.Protocol;

namespace PresentationTests.InternalPresentationMockups;

public class ClientMockup : IClient
{
    private Broker Broker;
    private List<Device> Devices;
    private List<Control> Controls;
    private bool _isConnected;

    public ClientMockup(Broker broker)
    {
        Broker = broker;
        Devices = new List<Device>();
        Controls = new List<Control>();
        _isConnected = false;

        //TODO: Mockup
        TopicService = new TopicService(null);
    }

    public string Id => Broker.Id;
    public bool IsConnected => _isConnected;

    public ITopicService TopicService { get; set; }
    public Func<Task> RerenderPageOnMessageReceived { get; set; }

    public Task<IResult> AddControl(Control control)
    {
        Controls.Add(control);
        return Task.FromResult((IResult) Result.Success());
    }

    public IResult AddDevice(Device device)
    {
        Devices.Add(device);
        return Result.Success();
    }

    public Task<IResult> AddDevices(Device device, List<Control> controls)
    {
        Devices.Add(device);
        Controls.AddRange(controls);
        return Task.FromResult((IResult) Result.Success());
    }

    public Task<IResult> ConnectAsync()
    {
        _isConnected = true;
        return Task.FromResult((IResult) Result.Success());
    }

    public Task DisconnectAsync()
    {
        _isConnected = false;
        return Task.FromResult((IResult) Result.Success());
    }

    public ValueTask DisposeAsync()
    {
        _isConnected = false;
        return new ValueTask();
    }

    public Broker GetBroker() => Broker;

    public IList<Control> GetControls(string deviceId)
    {
        return Controls.Where(x => x.DeviceId == deviceId).ToList();
    }

    public IList<Control> GetControls() => Controls;

    public IList<Device> GetDevices()
    {
        return Devices;
    }

    public bool HasDevice(string deviceId)
    {
        return Devices.Any(x => x.Id == deviceId);
    }

    public Task<IResult> PublishAsync(string topic, string payload, MqttQualityOfServiceLevel quality)
    {
        return Task.FromResult((IResult) Result.Success());
    }

    public Task<IResult> RemoveControl(string controlId)
    {
        Controls.RemoveAll(x => x.Id == controlId);
        return Task.FromResult((IResult) Result.Success());
    }

    public Task<IResult> RemoveDevice(string deviceId)
    {
        Devices.RemoveAll(x => x.Id == deviceId);
        return Task.FromResult((IResult) Result.Success());
    }

    public Task UpdateBroker(Broker broker)
    {
        Broker = broker;
        return Task.CompletedTask;
    }

    public Task<IResult> UpdateControl(Control control)
    {
        if(Controls.Any(x => x.Id == control.Id))
            Controls.FirstOrDefault(x => x.Id == control.Id)?.Update(control);
        else
            Controls.Add(control);

        return Task.FromResult((IResult) Result.Success());
    }

    public Task<IResult> UpdateDevice(Device device)
    {
        if(Devices.Any(x => x.Id == device.Id))
            Devices.FirstOrDefault(x => x.Id == device.Id)?.Update(device);
        else
            Devices.Add(device);

        return Task.FromResult((IResult) Result.Success());
    }

    public Task<IResult> UpdateDevice(Device device, List<Control> controls)
    {
        UpdateDevice(device);

        foreach(var control in controls)
            UpdateControl(control);

        return Task.FromResult((IResult) Result.Success());
    }
}
