namespace Presentation.Clients;

public interface IClientService
{
    Task Logout();

    Task<Result<List<IClient>>> GetClientsWithDevices();
    Task<Result<List<IClient>>> GetClients();
    Task<Result<IClient>> GetClient(string clientId);
    Task<Result> RemoveClient(string clientId);
    Task<Result<IClient>> CreateClient(BrokerDTO broker, BrokerCredentialsDTO brokerCredentialsDTO);
    Task<Result<IClient>> UpdateClient(BrokerDTO broker, BrokerCredentialsDTO brokerCredentialsDTO);

    Task<Result> RemoveDeviceFromClient(string clientId, Device device);
    Task<Result<Device>> CreateDeviceForClient(DeviceDTO device);
    Task<Result<Device>> UpdateDeviceForClient(DeviceDTO device);

    Task<Result> RemoveControlFromDevice(string clientId, string deviceId, Control control);
    Task<Result> CreateControlForDevice(string clientId, string deviceId, Control control);
    Task<Result> UpdateControlForDevice(string clientId, string deviceId, Control control);
}

