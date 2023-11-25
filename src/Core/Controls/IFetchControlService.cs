namespace Core.Controls;

public interface IFetchControlService
{
    Task<IResult<List<Control>>> GetControls(string deviceId);
    Task<IResult> RemoveControl(string deviceId, string controlId);
    Task<IResult<Control>> CreateControl(ControlDTO control);
    Task<IResult<Control>> UpdateControl(ControlDTO control);
}
