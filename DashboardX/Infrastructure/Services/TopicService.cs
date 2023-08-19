
using Blazored.LocalStorage;
using Core.Interfaces;
using Shared.Models.Controls;
using Shared.Models.Devices;

namespace Infrastructure.Services;

public class TopicService : ITopicService
{
    private readonly ILocalStorageService _localStorage;
    private readonly IDictionary<string, string> topics;

    public Func<Task> OnMessageReceived { get; set; }

    public TopicService(ILocalStorageService localStorage)
    {
        topics = new Dictionary<string, string>();
        _localStorage = localStorage;
        OnMessageReceived = default!;
    }

    public async Task RemoveTopic(string brokerId, Device device, Control control)
    {
        var identifier = Identifier(brokerId, device, control);

        topics.Remove(identifier);

        await _localStorage.RemoveItemAsync(identifier);
    }

    public async Task AddTopic(string brokerId, Device device, Control control)
    {
        var identifier = Identifier(brokerId, device, control);

        if (await _localStorage.ContainKeyAsync(identifier))
            topics[identifier] = await _localStorage.GetItemAsync<string>(identifier);
        else
            topics[identifier] = "";
    }

    public async Task UpdateMessageOnTopic(string brokerId, string topic, string message)
    {
        var identifier = Identifier(brokerId, topic);

        topics[identifier] = topic;

        await _localStorage.SetItemAsync(identifier, message);

        OnMessageReceived?.Invoke();
    }

    public async Task<string> LastMessageOnTopic(string brokerId, Device device, Control control)
    {
        var identifier = Identifier(brokerId, device, control);

        if (topics.ContainsKey(identifier))
            return topics[identifier];

        if (await _localStorage.ContainKeyAsync(identifier))
            return await _localStorage.GetItemAsync<string>(identifier);

        return string.Empty;
    }

    #region Privates    

    private string Identifier(string brokerId, Device device, Control control) => Identifier(brokerId, control.GetTopic(device));
    private string Identifier(string brokerId, string topic) => brokerId + topic;

    #endregion
}