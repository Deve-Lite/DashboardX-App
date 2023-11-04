
using MQTTnet.Client;
using MQTTnet.Protocol;

namespace PresentationTests.InternalPresentationMockups;

public class ClientMockup : IClient
{
    public string Id => Broker.Id;

    public bool IsConnected => true;

    public Broker Broker { get; private set; }

    public List<Device> Devices { get; private set; }

    public Func<Task>? RerenderPage { get; set; }

    public ITopicService TopicService => throw new NotImplementedException();


    public List<string> CurrentSubscribtions { get; private set; }  

    public ClientMockup(Broker broker)
    {
        Broker = broker;
        Devices = new List<Device>();
        CurrentSubscribtions = new List<string>();
    }


    public Task<Result> ConnectAsync()
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

    public Task<MqttClientPublishResult> PublishAsync(string topic, string payload, MqttQualityOfServiceLevel quality)
    {
        throw new NotImplementedException();
    }

    public Task<bool> Resubscibe(Device device, Control newControl)
    {
        throw new NotImplementedException();
    }

    public Task<int> SubscribeAsync(Device device, List<Control> data)
    {
        throw new NotImplementedException();
    }

    public Task<bool> SubscribeAsync(Device device, Control control)
    {
        throw new NotImplementedException();
    }

    public Task UnsubscribeAsync(Device existingDevice)
    {
        throw new NotImplementedException();
    }

    public Task UnsubscribeAsync(string deviceId, string id)
    {
        throw new NotImplementedException();
    }

    public Task UpdateBroker(Broker broker)
    {
        throw new NotImplementedException();
    }

    public Task<int> UpdateSubscribtionsAsync(string id, List<Control> data)
    {
        throw new NotImplementedException();
    }
}
