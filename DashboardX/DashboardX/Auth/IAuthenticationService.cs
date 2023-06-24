using DashboardXModels.Auth;
using DashboardXModels.Auth.DTO;

namespace DashboardX.Auth;

public interface IAuthenticationService
{
    Task<Response<TokenDTO>> Login(LoginData loginData);
    Task<Response> Register(RegisterData registerData);
}

