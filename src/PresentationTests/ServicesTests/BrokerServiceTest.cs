using Presentation.Brokers;
using Presentation.Brokers.Interfaces;

namespace PresentationTests.ServicesTests;

internal class BrokerServiceTest : BaseServiceTest
{
    public IBrokerService BrokerService { get; private set; }

    public BrokerServiceTest() : base()
    {
        BrokerService = new BrokerService(FetchBrokerService, ClientManager);
    }

    [SetUp]
    public override async Task SetUpTest()
    {
        await base.SetUpTest();
    }

    [TearDown]
    public void TearDown() 
    {
        base.TearDownTest();
        BrokerService = new BrokerService(FetchBrokerService, ClientManager);
    }

    [Test]
    public async Task CreateBrokerTest()
    {
        var clients = await ClientService.GetClientsWithDevices();

        Assert.That(clients!.Data, Has.Count.EqualTo(2));

        var newBroker = BrokerDtoGenerator.GenerateBrokerDto();
        var result = await BrokerService.CreateBroker(newBroker, BrokerDtoGenerator.GenerateBrokerCredentialsDto());

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Succeeded);

        clients = await ClientService.GetClients();

        Assert.That(clients.Data, Has.Count.EqualTo(3));

    }

    [Test]
    public async Task UpdateBrokerTest()
    {
        var clients = await ClientService.GetClientsWithDevices();

        Assert.That(clients!.Data, Has.Count.EqualTo(2));

        var brokers = await FetchBrokerService.GetBrokers();
        var broker = brokers.Data[0];
        broker.Server = "google.com";
        var result = await BrokerService.UpdateBroker(broker.Dto(), BrokerDtoGenerator.GenerateBrokerCredentialsDto());

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Succeeded);

        clients = await ClientService.GetClients();

        Assert.That(clients.Data, Has.Count.EqualTo(2));
        Assert.That(clients.Data.Any(x => x.GetBroker().Server == "google.com"), Is.True);
    }

    [Test]
    public async Task RemoveBrokerTest()
    {
        var clients = await ClientService.GetClientsWithDevices();

        Assert.That(clients!.Data, Has.Count.EqualTo(2));

        var brokers = await FetchBrokerService.GetBrokers();
        var brokerToRemove = brokers.Data[0];

        var result = await BrokerService.RemoveBroker(brokerToRemove.Id);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Succeeded);

        clients = await ClientService.GetClients();

        Assert.That(clients.Data, Has.Count.EqualTo(1));
    }
}
