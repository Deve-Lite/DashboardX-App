namespace Presentation.Clients;

public interface IClientService
{
    Task Logout();

    Task<Result<List<IClient>>> GetClientsWithDevices();
    Task<Result<List<IClient>>> GetClients();
    Task<Result<IClient>> GetClient(string clientId);
    Task<Result> RemoveClient(string clientId);
    Task<IResult> CreateClient(BrokerDTO broker, BrokerCredentialsDTO brokerCredentialsDTO);
    Task<IResult> UpdateClient(BrokerDTO broker, BrokerCredentialsDTO brokerCredentialsDTO);

    Task<IResult> RemoveDevice(string clientId, string deviceId);
    Task<IResult> CreateDevice(DeviceDTO device);
    Task<IResult> UpdateDevice(DeviceDTO device);

    Task<IResult> RemoveControl(string clientId, Control control);
    Task<IResult> CreateControlForDevice(string clientId, Control control);
    Task<IResult> UpdateControlForDevice(string clientId, Control control);
}

