namespace PresentationTests.ServiceMockups;


internal class FetchControlServiceMockup : IFetchControlService
{

    private List<Control> Controls { get; set; } = new();

    public Task<IResult<Control>> UpdateControl(ControlDto control)
    {
        var oldControl = Controls.First(x => x.Id == control.Id);

        oldControl.Update(new Control(control.Id, control));

        return Task.FromResult((IResult<Control>)Result<Control>.Success(oldControl));
    }

    public Task<IResult<Control>> CreateControl(ControlDto control)
    {
        var copy = new Control(control.Id, control);
        Controls.Add(copy);
        return Task.FromResult((IResult<Control>)Result<Control>.Success(copy));
    }

    public Task<IResult<List<Control>>> GetControls(string deviceId)
    {
        return Task.FromResult((IResult<List<Control>>)Result<List<Control>>.Success(Controls.Where(x => x.DeviceId == deviceId).ToList()));
    }

    public Task<IResult> RemoveControl(string deviceId, string controlId)
    {
        Controls.RemoveAll(x => x.DeviceId == deviceId && x.Id == controlId);
        return Task.FromResult((IResult)Result.Success());
    }
}
