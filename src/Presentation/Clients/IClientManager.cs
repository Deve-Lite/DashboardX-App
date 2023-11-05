namespace Presentation.Clients;

public interface IClientManager
{
    IResult<IClient> GetClient(string clientId);
    IResult<IList<IClient>> GetClients();
    IResult<IClient> AddClient(Broker broker);
    Task<IResult<IClient>> UpdateClient(Broker broker);
    Task<IResult> RemoveClient(string clientId);
}
