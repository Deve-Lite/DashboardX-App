using MQTTnet.Client;
using MQTTnet.Protocol;

namespace Presentation.Clients;

public interface IClient
{
    string Id { get; }
    bool IsConnected { get; }

    Broker Broker { get; }
    List<Device> Devices { get; }
    Func<Task> RerenderPage { get; set; }

    ITopicService TopicService { get; }

    Task DisconnectAsync();
    ValueTask DisposeAsync(); 
    Task<Result> ConnectAsync();
    Task<bool> Resubscibe(Device device, Control newControl);
    Task<int> SubscribeAsync(Device device, List<Control> data);
    Task<bool> SubscribeAsync(Device device, Control control);
    Task UnsubscribeAsync(Device existingDevice);
    Task UnsubscribeAsync(string deviceId, string id);
    Task UpdateBroker(Broker broker);
    Task<int> UpdateSubscribtionsAsync(string id, List<Control> data);
    Task<MqttClientPublishResult> PublishAsync(string topic, string payload, MqttQualityOfServiceLevel quality);
}