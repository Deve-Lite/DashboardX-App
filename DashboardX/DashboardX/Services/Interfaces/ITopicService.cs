using DashboardXModels.Controls;
using DashboardXModels.Devices;

namespace DashboardX.Services.Interfaces;

public interface ITopicService
{
    Task AddTopic(string brokerId, Device device, Control control);
    Task RemoveTopic(string brokerId, Device device, Control control);
    Task UpdateTopic(string brokerId, string topic, string message);
    Task<string> GetMessage(string brokerId, Device device, Control control);
}