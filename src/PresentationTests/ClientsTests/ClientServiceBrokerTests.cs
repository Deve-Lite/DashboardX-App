using Microsoft.Extensions.Logging;

namespace PresentationTests.ClientsTests;

public class ClientServiceBrokerTests
{
    public IBrokerService BrokerService { get; private set; }
    public IDeviceService DeviceService { get; private set; }
    public IClientService ClientService { get; private set; }
    public IClientFactory ClientFactory { get; private set; }
    public ILogger<ClientService> Logger { get; private set; }

    public ClientServiceBrokerTests()
    {
        BrokerService = new BrokerServiceMockup();
        DeviceService = new DeviceServiceMockup();
        ClientFactory = new ClientFactoryMockup();
        Logger = new Logger<ClientService>(new LoggerFactory());

        ClientService = new ClientService(BrokerService, DeviceService, Logger, ClientFactory);
    }

    [SetUp]
    public async Task SetUpTest()
    {
        await BrokerService.CreateBroker(BrokerDtoGenerator.FirstBroker());
        await BrokerService.CreateBroker(BrokerDtoGenerator.SecondBroker());

        await DeviceService.CreateDevice(DeviceDtoGenerator.FirstDevice());
        await DeviceService.CreateDevice(DeviceDtoGenerator.SecondDevice());

        var listOfControls = new List<List<Control>>()
        {
            new List<Control>()
            {
                ControlGenerator.FirstDeviceControl1(),
                ControlGenerator.FirstDeviceControl2(),
                ControlGenerator.FirstDeviceControl3(),
            },
            new List<Control>()
            {
                ControlGenerator.SecondDeviceControl1(),
                ControlGenerator.SecondDeviceControl2(),
                ControlGenerator.SecondDeviceControl3(),
            }
        };

        foreach (var sublist in listOfControls)
            foreach (var control in sublist)
                await DeviceService.CreateDeviceControl(control);
    }

    [TearDown]
    public void TearDownTest()
    {
        BrokerService = new BrokerServiceMockup();
        DeviceService = new DeviceServiceMockup();
        ClientFactory = new ClientFactoryMockup();
        Logger = new Logger<ClientService>(new LoggerFactory());
        ClientService = new ClientService(BrokerService, DeviceService, Logger, ClientFactory);
    }

    [Test]
    public async Task GetClientsTest()
    {
        var clients = await ClientService.GetClients();

        Assert.That(clients!.Data, Has.Count.EqualTo(2));

        var brokers = await BrokerService.GetBrokers();
        var brokersInClients = clients.Data
                                     .Select(x => x.Broker)
                                     .ToList();

        foreach (var broker in brokers.Data)
            Assert.That(brokersInClients, Does.Contain(broker));
    }

    [Test]
    public async Task GetClientTest()
    {
        var clients = await ClientService.GetClients();

        Assert.That(clients.Data.Count, Is.EqualTo(2));

        var brokers = await BrokerService.GetBrokers();

        foreach (var broker in brokers.Data)
        {
            var result = await ClientService.GetClient(broker.Id);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Succeeded, Is.True);
            Assert.That(result.Data, Is.Not.Null);
            Assert.That(result.Data.Broker.Id, Is.EqualTo(broker.Id));
        }

    }

    [Test]
    public async Task AddNewClientTest()
    {
        var clients = await ClientService.GetClients();

        Assert.That(clients.Data.Count, Is.EqualTo(2));

        var result = await BrokerService.CreateBroker(BrokerDtoGenerator.GenerateBrokerDto());

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Succeeded, Is.True);

        var resultClients = await ClientService.GetClients();
        var brokers = await BrokerService.GetBrokers();

        Assert.That(resultClients.Data.Count, Is.EqualTo(3));

        var brokersInClients = clients.Data
                             .Select(x => x.Broker)
                             .ToList();

        foreach (var broker in brokers.Data)
            Assert.That(brokersInClients, Does.Contain(broker));
    }

    [Test]
    public async Task RemoveClientsTest()
    {
        var brokers = await BrokerService.GetBrokers();

        var firstToRemove = brokers.Data[0];
        var secondToRemove = brokers.Data[1];

        var clients = await ClientService.GetClients();
        Assert.That(clients.Data, Has.Count.EqualTo(2));

        var result = await BrokerService.RemoveBroker(firstToRemove.Id);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Succeeded, Is.True);

        var resultClients = await ClientService.GetClients();

        Assert.That(resultClients.Data, Has.Count.EqualTo(1));

        result = await BrokerService.RemoveBroker(secondToRemove.Id);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Succeeded, Is.True);

        resultClients = await ClientService.GetClients();

        Assert.That(resultClients.Data, Is.Empty);
    }

    [Test]
    public async Task UpdateClientTest()
    {
        var clients = await ClientService.GetClients();

        Assert.That(clients.Data.Count, Is.EqualTo(2));

        var result = await BrokerService.GetBroker(clients.Data[0].Id);

        Assert.That(result.Succeeded, Is.True);

        var updatedDto = BrokerDtoGenerator.GenerateBrokerDto();

        updatedDto.Id = result.Data.Id;
        updatedDto.Port = result.Data.Port;
        updatedDto.KeepAlive = result.Data.KeepAlive;

        var updateResult = await BrokerService.UpdateBroker(updatedDto);   

        Assert.That(updateResult.Succeeded, Is.True);

        clients = await ClientService.GetClients();

        Assert.That(clients.Data.Count, Is.EqualTo(2));

        var updatedClient = clients.Data
            .Where(x => x.Broker.Id == result.Data.Id)
            .FirstOrDefault();

        Assert.That(updatedClient, Is.Not.Null);
        Assert.That(updatedClient.Broker.Id, Is.EqualTo(updateResult.Data.Id));
        Assert.That(updatedClient.Broker.Port, Is.EqualTo(updateResult.Data.Port));
        Assert.That(updatedClient.Broker.KeepAlive, Is.EqualTo(updateResult.Data.KeepAlive));
        Assert.That(updatedClient.Broker.Server, Is.EqualTo(updateResult.Data.Server));
    }
}