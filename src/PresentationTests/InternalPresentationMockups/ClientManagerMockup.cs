using PresentationTests.InternalPresentationMockups;

namespace PresentationTests.InternalServiceMockups;

internal class ClientManagerMockup : IClientManager
{
    private readonly List<IClient> _clients = new();

    public IResult<IClient> AddClient(Broker broker)
    {
        var client = new ClientMockup(broker);
        _clients.Add(client);
        return Result<IClient>.Success(client);
    }

    public IResult<IClient> GetClient(string clientId)
    {
        if (!_clients.Any(c => c.Id == clientId))
            return Result<IClient>.Fail();

        return Result<IClient>.Success(_clients.First(c => c.Id == clientId));
    }

    public IResult<IList<IClient>> GetClients()
    {
        return Result<IList<IClient>>.Success(_clients);
    }

    public Task<IResult> RemoveClient(string clientId)
    {
        _clients.RemoveAll(c => c.Id == clientId);
        IResult sucess = Result.Success();
        return Task.FromResult(sucess);
    }

    public Task<IResult<IClient>> UpdateClient(Broker broker)
    {
        var client = _clients.First(c => c.Id == broker.Id);
        client.UpdateBroker(broker);
        return Task.FromResult((IResult<IClient>)Result<IClient>.Success(client));
    }
}
