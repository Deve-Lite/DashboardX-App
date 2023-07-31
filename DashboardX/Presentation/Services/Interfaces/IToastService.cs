namespace Presentation.Services.Interfaces;

public interface IToastService
{
    Task Success(string successMessage);
    Task Warning(string warningMessage);
    Task Error(string successMessage);
    Task Info(string message);
}
