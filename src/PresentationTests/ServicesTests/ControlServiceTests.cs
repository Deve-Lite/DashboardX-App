using Presentation.Brokers;
using Presentation.Devices;
using Presentation.Controls.Interfaces;
using Presentation.Controls;

namespace PresentationTests.ServicesTests;

internal class ControlServiceTests : BaseServiceTest
{
    public IControlService ControlService { get; private set; }

    public ControlServiceTests() : base()
    {
        ControlService = new ControlService(ClientManager, FetchControlService);
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
        ControlService = new ControlService(ClientManager, FetchControlService);
    }

    [Test]
    public async Task CreateControlTest()
    {
        var clients = await ClientService.GetClientsWithDevices();

        Assert.That(clients!.Data, Has.Count.EqualTo(2));

        var devices = await FetchDeviceService.GetDevices();
        var controlDto = ControlGenerator.GenerateControl();
        controlDto.DeviceId = devices.Data[0].Id;

        var result = await ControlService.CreateControl(devices.Data[0].BrokerId, controlDto);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Succeeded);

        clients = await ClientService.GetClientsWithDevices();

        var totalControls = clients.Data.SelectMany(x => x.GetControls())
            .ToList();

        Assert.That(totalControls, Has.Count.EqualTo(7));
    }

    [Test]
    public async Task UpdateControlTest()
    {
        var clients = await ClientService.GetClientsWithDevices();

        Assert.That(clients!.Data, Has.Count.EqualTo(2));

        var device = clients.Data[0].GetDevices()[0];
        var control = clients.Data[0].GetControls(device.Id)[0];

        var newDisplayName = "NewDisplayName";

        control.Name = newDisplayName;

        var result = await ControlService.UpdateControl(device.BrokerId, control);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Succeeded);

        clients = await ClientService.GetClientsWithDevices();

        var totalDevices = clients.Data.SelectMany(x => x.GetControls())
            .ToList();

        Assert.That(totalDevices, Has.Count.EqualTo(6));
        Assert.That(totalDevices.Any(x => x.Name==newDisplayName), Is.True);
    }

    [Test]
    public async Task RemoveControlTest()
    {
        var clients = await ClientService.GetClientsWithDevices();

        Assert.That(clients!.Data, Has.Count.EqualTo(2));

        var device = clients.Data[0].GetDevices()[0];
        var control = clients.Data[0].GetControls(device.Id)[0];

        var result = await ControlService.RemoveControl(device.BrokerId, control);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Succeeded);

        clients = await ClientService.GetClientsWithDevices();

        var totalControls = clients.Data.SelectMany(x => x.GetControls())
            .ToList();

        Assert.That(totalControls, Has.Count.EqualTo(5));
    }
}
