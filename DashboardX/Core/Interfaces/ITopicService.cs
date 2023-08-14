using Shared.Models.Controls;
using Shared.Models.Devices;

namespace Core.Interfaces;

public interface ITopicService
{
    Task AddTopic(string brokerId, Device device, Control control);
    Task RemoveTopic(string brokerId, Device device, Control control);
    Task UpdateMessageOnTopic(string brokerId, string topic, string message);
    Task<string> LastMessageOnTopic(string brokerId, Device device, Control control);
}
