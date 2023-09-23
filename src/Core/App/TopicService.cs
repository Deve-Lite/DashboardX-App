using Blazored.LocalStorage;

namespace Core.App;

/// <summary>
/// TODO: Create class in client not as dependency injection
/// </summary>
public class TopicService : ITopicService
{
    private readonly ILocalStorageService _localStorage;
    private readonly IDictionary<string, string> topics;

    public List<(string, string)> Topics => topics.Select(x => (x.Key, x.Value)).ToList();

    public Func<Task> OnMessageReceived { get; set; }

    public TopicService(ILocalStorageService localStorage)
    {
        topics = new Dictionary<string, string>();
        _localStorage = localStorage;
        OnMessageReceived = default!;
    }

    public async Task<string> RemoveTopic(string brokerId, Device device, Control control)
    {
        var topic = GetTopic(device,control);
        topics.Remove(topic);

        var identifier = Identifier(brokerId, topic);
        await _localStorage.RemoveItemAsync(identifier);

        return identifier;
    }

    public async Task<string> AddTopic(string brokerId, Device device, Control control)
    {
        var topic = GetTopic(device, control);

        if (topics.ContainsKey(topic))
            return topic;

        var identifier = Identifier(brokerId, topic);

        if (await _localStorage.ContainKeyAsync(identifier))
            topics[topic] = await _localStorage.GetItemAsync<string>(identifier);
        else
            topics[topic] = "";

        return topic;
    }

    public async Task UpdateMessageOnTopic(string brokerId, string topic, string message)
    {
        topics[topic] = message;

        var identifier = Identifier(brokerId, topic);
        await _localStorage.SetItemAsync(identifier, message);

        OnMessageReceived?.Invoke();
    }

    public async Task UpdateMessageOnTopic(string brokerId, Device device, Control control, string message)
    {
        var topic = GetTopic(device, control);
        topics[topic] = message;

        var identifier = Identifier(brokerId, topic);
        await _localStorage.SetItemAsync(identifier, message);

        OnMessageReceived?.Invoke();
    }

    public async Task<string> LastMessageOnTopicAsync(string brokerId, Device device, Control control)
    {
        var topic = GetTopic(device, control);

        if (topics.ContainsKey(topic))
            return topics[topic];

        var identifier = Identifier(brokerId, device, control);

        if (await _localStorage.ContainKeyAsync(identifier))
            return await _localStorage.GetItemAsync<string>(identifier);

        return string.Empty;
    }

    public string LastMessageOnTopic(string brokerId, Device device, Control control)
    {
        var topic = GetTopic(device, control);

        if (topics.ContainsKey(topic))
            return topics[topic];

        return string.Empty;
    }

    public bool ConatinsTopic(string brokerId, Device device, Control control) => topics.ContainsKey(Identifier(brokerId, device, control));

    #region Privates    
    private string GetTopic(Device device, Control control) => control.GetTopic(device);
    private string Identifier(string brokerId, Device device, Control control) => Identifier(brokerId, control.GetTopic(device));
    private string Identifier(string brokerId, string topic) => $"{brokerId}/{topic}";

    #endregion
}