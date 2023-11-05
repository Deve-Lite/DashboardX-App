using MQTTnet.Protocol;

namespace Presentation.Clients;

public interface IClient
{
    string Id { get; }
    bool IsConnected { get; }

    Broker Broker { get; }
    IList<Device> Devices { get; }
    Func<Task> RerenderPage { get; set; }
    ITopicService TopicService { get; }

    Task UpdateBroker(Broker broker);
    Task<IResult> AddControl(Control control);
    Task<IResult> RemoveControl(string controlId);
    Task<IResult> UpdateControl(Control control);
    IList<Control> GetControls(string deviceId);

    IResult AddDevice(Device device);
    Task<IResult> AddDevices(Device device, List<Control> controls);
    Task<IResult> RemoveDevice(string deviceId);
    Task<IResult> UpdateDevice(Device device);
    Task<IResult> UpdateDevice(Device device, List<Control> controls);
    bool HasDevice(string deviceId);

    Task<IResult> ConnectAsync();
    Task<IResult> PublishAsync(string topic, string payload, MqttQualityOfServiceLevel quality);
    Task DisconnectAsync();
    ValueTask DisposeAsync();
}