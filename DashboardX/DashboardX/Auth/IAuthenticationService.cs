using DashboardXModels.Auth;

namespace DashboardX.Auth;

public interface IAuthenticationService
{
    Task<Response> Login(LoginData loginData);
    Task<Response> Register(RegisterData registerData);
}

