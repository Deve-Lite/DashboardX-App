using Presentation.Controls.Interfaces;
using Presentation.Controls;

namespace PresentationTests;

public class ControlServiceTests : BaseTest, IAsyncLifetime
{
    public IControlService? ControlService { get; private set; }

    public async Task InitializeAsync()
    {
        ControlService = new ControlService(ClientManager, FetchControlService);
        await Setup();
    }

    public Task DisposeAsync()
    {
        TearDown();
        return Task.CompletedTask;
    }

    [Fact]
    public async Task CreateControlTest()
    {
        var clients = await ClientService.GetClientsWithDevices();

        Assert.Equal(2, clients!.Data.Count);

        var devices = await FetchDeviceService.GetDevices();
        var controlDto = ControlGenerator.GenerateControl();
        controlDto.DeviceId = "1";

        var result = await ControlService!.CreateControl(devices.Data[0].BrokerId, controlDto);

        Assert.NotNull(result);
        Assert.True(result.Succeeded);

        clients = await ClientService.GetClientsWithDevices();

        var totalControls = clients.Data.SelectMany(x => x.GetControls()).ToList();

        Assert.Equal(7, totalControls.Count);
    }

    [Fact]
    public async Task UpdateControlTest()
    {
        var clients = await ClientService.GetClientsWithDevices();

        Assert.Equal(2, clients!.Data.Count);

        var device = clients.Data[0].GetDevices()[0];
        var control = clients.Data[0].GetControls(device.Id)[0];

        var newDisplayName = "NewDisplayName";

        control.Name = newDisplayName;

        var result = await ControlService!.UpdateControl(device.BrokerId, control.Dto());

        Assert.NotNull(result);
        Assert.True(result.Succeeded);

        clients = await ClientService.GetClientsWithDevices();

        var totalControls = clients.Data.SelectMany(x => x.GetControls()).ToList();

        Assert.Equal(6, totalControls.Count);
        Assert.Contains(totalControls, x => x.Name == newDisplayName);
    }

    [Fact]
    public async Task RemoveControlTest()
    {
        var clients = await ClientService.GetClientsWithDevices();

        Assert.Equal(2, clients!.Data.Count);

        var device = clients.Data[0].GetDevices()[0];
        var control = clients.Data[0].GetControls(device.Id)[0];

        var result = await ControlService!.RemoveControl(device.BrokerId, control);

        Assert.NotNull(result);
        Assert.True(result.Succeeded);

        clients = await ClientService.GetClientsWithDevices();

        var totalControls = clients.Data.SelectMany(x => x.GetControls())
            .ToList()
            .Select(x => x.Id);

        Assert.DoesNotContain(control.Id, totalControls);
    }
}
