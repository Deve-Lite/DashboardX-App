namespace DashboardX.Auth;

public interface IAuthenticationService
{
    Task<Response> Login(string username, string password);
    Task<Response> Register(string username, string email, string password);
}

