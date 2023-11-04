namespace PresentationTests.ServiceMockups;

internal class DeviceServiceMockup : IDeviceService
{
    private List<Device> Devices { get; set; } = new();
    private List<Control> Controls { get; set; } = new();

    public Task<IResult<Device>> CreateDevice(DeviceDTO dto)
    {
        var device = new Device
        {
            Id = Guid.NewGuid().ToString(),
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

    public Task<IResult<Control>> CreateDeviceControl(Control control)
    {
        Controls.Add(control);
        return Task.FromResult((IResult<Control>)Result<Control>.Success(control));
    }

    public Task<IResult<Device>> GetDevice(string id)
    {
        return Task.FromResult((IResult<Device>)Result<Device>.Success(Devices.First(x => x.Id == id)));
    }

    public Task<IResult<List<Control>>> GetDeviceControls(string deviceId)
    {
        return Task.FromResult((IResult<List<Control>>)Result<List<Control>>.Success(Controls.Where(x => x.DeviceId == deviceId).ToList()));
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

    public Task<IResult> RemoveDeviceControl(string deviceId, string controlId)
    {
        Controls.RemoveAll(x => x.DeviceId == deviceId && x.Id == controlId);
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

    public Task<IResult<Control>> UpdateDeviceControl(Control control)
    {
        var oldControl = Controls.First(x => x.Id == control.Id);

        oldControl.Name = control.Name;
        oldControl.Icon = control.Icon; 
        oldControl.DeviceId = control.DeviceId;
        oldControl.Type = control.Type;
        oldControl.Attributes = control.Attributes;
        oldControl.NotifyOnPublish = control.NotifyOnPublish;
        oldControl.DisplayName = control.DisplayName;
        oldControl.Topic = control.Topic;
        oldControl.QualityOfService = control.QualityOfService;
        oldControl.IsConfiramtionRequired = control.IsConfiramtionRequired;
        oldControl.IsAvailable = control.IsAvailable;

        return Task.FromResult((IResult<Control>)Result<Control>.Success(oldControl));
    }
}
