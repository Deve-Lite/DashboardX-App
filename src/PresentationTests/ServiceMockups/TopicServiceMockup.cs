namespace PresentationTests.ServiceMockups;

internal class TopicServiceMockup : ITopicService
{
    public List<(string, string)> Topics { get; set; }

    public Func<Task>? OnMessageReceived { get; set; }

    public TopicServiceMockup()
    {
        Topics = new();
    }

    public Task<string> AddTopic(string brokerId, Device device, Control control)
    {
        var topic = Topic(brokerId, device, control);

        Topics.Add((topic, ""));

        return Task.FromResult(topic);
    }

    public bool ConatinsTopic(string brokerId, Device device, Control control)
    {
        return Topics.Contains((Topic(brokerId, device, control), ""));
    }

    public string LastMessageOnTopic(string brokerId, Device device, Control control)
    {
        return Topics.FirstOrDefault(t => t.Item1 == Topic(brokerId, device, control)).Item2;
    }

    public Task<string> LastMessageOnTopicAsync(string brokerId, Device device, Control control)
    {
        return Task.FromResult(LastMessageOnTopic(brokerId,device,control));
    }

    public Task<string> RemoveTopic(string brokerId, Device device, Control control)
    {
        var topic = Topic(brokerId, device, control);
        Topics.Remove((topic, ""));
        return Task.FromResult(topic);
    }

    public Task UpdateMessageOnTopic(string brokerId, string topic, string message)
    {
        throw new NotImplementedException();
    }

    public Task UpdateMessageOnTopic(string brokerId, Device device, Control control, string message)
    {
        throw new NotImplementedException();
    }
        
    private string Topic(string brokerId, Device device, Control control)
    {
        return $"{brokerId}/{device.Id}{control.Id}";
    }
}
