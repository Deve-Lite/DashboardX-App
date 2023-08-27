using Shared.Models.Controls;
using Shared.Models.Devices;

namespace Core.Interfaces;

public interface ITopicService
{
    Func<Task> OnMessageReceived { get; set; }
    Task<string> AddTopic(string brokerId, Device device, Control control);
    Task<string> RemoveTopic(string brokerId, Device device, Control control);
    Task UpdateMessageOnTopic(string brokerId, string topic, string message);
    Task<string> LastMessageOnTopic(string brokerId, Device device, Control control);
}
