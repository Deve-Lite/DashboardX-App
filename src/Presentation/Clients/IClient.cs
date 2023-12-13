using MQTTnet.Protocol;

namespace Presentation.Clients;

public interface IClient
{
    string Id { get; }
    bool IsConnected { get; }
    ITopicService TopicService { get; }

    public void SetOnMessageReceivedEventHandler(Func<Task> refreshAction);
    public void ClearOnMessageReceivedEventHandler();

    Broker GetBroker();
    Task UpdateBroker(Broker broker);

    Task<IResult> AddControl(Control control);
    Task<IResult> RemoveControl(string controlId);
    Task<IResult> UpdateControl(Control control);
    IList<Control> GetControls(string deviceId);
    IList<Control> GetControls();

    IList<Device> GetDevices();
    IResult AddDevice(Device device);
    Task<IResult> AddDevice(Device device, IList<Control> controls);
    Task<IResult> RemoveDevice(string deviceId);
    Task<IResult> UpdateDevice(Device device);
    Task<IResult> UpdateDevice(Device device, IList<Control> controls);
    bool HasDevice(string deviceId);

    Task<IResult> ConnectAsync();
    Task<IResult> PublishAsync(string topic, string payload, MqttQualityOfServiceLevel quality);
    Task DisconnectAsync();
    ValueTask DisposeAsync();
}