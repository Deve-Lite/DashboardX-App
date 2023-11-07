namespace PresentationTests.ServiceMockups;


internal class FetchControlServiceMockup : IFetchControlService
{

    private List<Control> Controls { get; set; } = new();

    public Task<IResult<Control>> UpdateControl(Control control)
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

    public Task<IResult<Control>> CreateControl(Control control)
    {
        var copy = control.Copy();
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
