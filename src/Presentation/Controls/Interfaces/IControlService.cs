namespace Presentation.Controls.Interfaces;

public interface IControlService
{
    Task<IResult> RemoveControl(string clientId, Control control);
    Task<IResult> CreateControl(string clientId, ControlDTO control);
    Task<IResult> UpdateControl(string clientId, ControlDTO control);
}
