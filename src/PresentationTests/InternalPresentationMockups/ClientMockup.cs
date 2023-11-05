
using MQTTnet.Client;
using MQTTnet.Protocol;

namespace PresentationTests.InternalPresentationMockups;

public class ClientMockup : IClient
{
    public string Id => Broker.Id;

    public bool IsConnected => true;

    public Broker Broker { get; private set; }
    public Func<Task> RerenderPage { get; set; } = default;
    public ITopicService TopicService { get; private set; }
    public List<Control> Controls { get; private set; }
    public IList<Device> Devices { get; private set; }

    public ClientMockup(Broker broker)
    {
        Broker = broker;
        Devices = new List<Device>();
    }

    public Task UpdateBroker(Broker broker)
    {
        throw new NotImplementedException();
    }

    public Task<IResult> AddControl(Control control)
    {
        throw new NotImplementedException();
    }

    public Task<IResult> RemoveControl(string controlId)
    {
        throw new NotImplementedException();
    }

    public Task<IResult> UpdateControl(Control control)
    {
        throw new NotImplementedException();
    }

    public IResult AddDevice(Device device)
    {
        throw new NotImplementedException();
    }

    public Task<IResult> AddDevices(Device device, List<Control> controls)
    {
        throw new NotImplementedException();
    }

    public Task<IResult> RemoveDevice(string deviceId)
    {
        throw new NotImplementedException();
    }

    public Task<IResult> UpdateDevice(Device device)
    {
        throw new NotImplementedException();
    }

    public Task<IResult> UpdateDevice(Device device, List<Control> controls)
    {
        throw new NotImplementedException();
    }

    public IList<Control> GetControls(string deviceId)
    {
        throw new NotImplementedException();
    }

    public bool HasDevice(string deviceId)
    {
        throw new NotImplementedException();
    }

    public Task<IResult> ConnectAsync()
    {
        throw new NotImplementedException();
    }

    public Task<IResult> PublishAsync(string topic, string payload, MqttQualityOfServiceLevel quality)
    {
        throw new NotImplementedException();
    }

    public Task DisconnectAsync()
    {
        throw new NotImplementedException();
    }

    public ValueTask DisposeAsync()
    {
        throw new NotImplementedException();
    }
}
