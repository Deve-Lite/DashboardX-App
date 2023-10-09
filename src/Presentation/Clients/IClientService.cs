namespace Presentation.Clients;

public interface IClientService
{
    Task Logout();

    Task<Result<List<Client>>> GetClientsWithDevices();
    Task<Result<List<Client>>> GetClients();
    Task<Result<Client>> GetClient(string clientId);
    Task<Result> RemoveClient(string clientId);
    Task<Result<Client>> CreateClient(BrokerDTO broker, BrokerCredentialsDTO brokerCredentialsDTO);
    Task<Result<Client>> UpdateClient(BrokerDTO broker, BrokerCredentialsDTO brokerCredentialsDTO);

    Task<Result> RemoveDeviceFromClient(string clientId, Device device);
    Task<Result<Device>> CreateDeviceForClient(DeviceDTO device);
    Task<Result<Device>> UpdateDeviceForClient(DeviceDTO device);

    Task<Result> RemoveControlFromDevice(string clientId, string deviceId, Control control);
    Task<Result> CreateControlForDevice(string clientId, string deviceId, Control control);
    Task<Result> UpdateControlForDevice(string clientId, string deviceId, Control control);
}

