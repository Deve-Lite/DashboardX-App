namespace PresentationTests.ClientsTests;

public class ClientServiceTests : IClassFixture<ClientServiceTextFixture>
{

    private readonly ClientServiceTextFixture _fixture;

    public ClientServiceTests(ClientServiceTextFixture fixture)
    {
        _fixture = fixture;


    }

    [Fact]
    public void GetClientsTest()
    {
        var result = _fixture.ClientService.GetClients();

    }
}