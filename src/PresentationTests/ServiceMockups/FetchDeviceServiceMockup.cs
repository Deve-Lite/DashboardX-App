namespace PresentationTests.ServiceMockups;

internal class FetchDeviceServiceMockup : IFetchDeviceService
{
    private List<Device> Devices { get; set; } = new();

    public Task<IResult<Device>> CreateDevice(DeviceDTO dto)
    {
        var device = new Device
        {
            Id = dto.Id,
            BrokerId = dto.BrokerId,
            Name = dto.Name,
            BaseDevicePath = dto.BaseDevicePath,
            Icon = dto.Icon.Copy(),
            Placing = dto.Placing,
            EditedAt = DateTime.Now
        };

        Devices.Add(device);

        return Task.FromResult((IResult<Device>)Result<Device>.Success(device));
    }

    public Task<IResult<Device>> GetDevice(string id)
    {
        return Task.FromResult((IResult<Device>)Result<Device>.Success(Devices.First(x => x.Id == id)));
    }

    public Task<IResult<List<Device>>> GetDevices()
    {
        return Task.FromResult((IResult<List<Device>>)Result<List<Device>>.Success(Devices));
    }

    public Task<IResult<List<Device>>> GetDevices(string brokerId)
    {
        return Task.FromResult((IResult<List<Device>>)Result<List<Device>>.Success(Devices.Where(x => x.BrokerId == brokerId).ToList()));
    }

    public Task<IResult> RemoveDevice(string deviceId)
    {
        Devices.RemoveAll(x => x.Id == deviceId);
        return Task.FromResult((IResult)Result.Success());
    }

    public Task<IResult<Device>> UpdateDevice(DeviceDTO dto)
    {
        var device = Devices.First(x => x.Id == dto.Id);

        device.Name = dto.Name;
        device.Icon = dto.Icon;
        device.Placing = dto.Placing;
        device.BaseDevicePath = dto.BaseDevicePath;
        device.EditedAt = DateTime.Now;
        device.BrokerId = dto.BrokerId;

        return Task.FromResult((IResult<Device>)Result<Device>.Success(device));
    }
}
