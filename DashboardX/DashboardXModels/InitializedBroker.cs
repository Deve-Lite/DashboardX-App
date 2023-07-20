using MQTTnet.Client;

namespace DashboardXModels;

public class InitializedBroker
{
    public InitializedBroker(IMqttClient client, Broker broker)
    {
        Client = client;
        Broker = broker;
        Topics  = new Dictionary<string, string>();
        Devices = new Dictionary<string, Device>();
    }

    public IMqttClient Client { get; set; }
    public Broker Broker { get; set; }
    public IDictionary<string,string > Topics { get; set; }
    public IDictionary<string, Device> Devices { get; set; }
}
