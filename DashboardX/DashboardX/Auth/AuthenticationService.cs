using DashboardX.Services;
using DashboardX.Services.Interfaces;
using DashboardXModels.Auth;
using DashboardXModels.Auth.DTO;

namespace DashboardX.Auth;

public class AuthenticationService : BaseService, IAuthenticationService
{
    private readonly IAuthorizationService _authorizationService;

    public AuthenticationService(HttpClient httpClient, 
                                 IAuthorizationService authorizationService, 
                                 IConfiguration configuration) : base(httpClient, configuration)
    {
        _authorizationService = authorizationService;
    }

    public async Task<Response<TokenDTO>> Login(LoginData data)
    {
        var loginDto = new LoginDTO
        {
            Email = data.Email,
            Password = data.Password
        };

        var request = new Request
        {
            Method = HttpMethod.Post,
            Route = "users/login",
            Data = loginDto
        };

        var response = await SendAsync<TokenDTO>(request);

        if (response.Success)
        {
            _authorizationService.AuthenticateSession(response.Data.AccessToken, response.Data.RefreshToken);

            return new Response<TokenDTO>
            { 
                StatusCode = response.StatusCode,
                Data = response.Data
            };
        }

        return new Response<TokenDTO>
        { 
            StatusCode = response.StatusCode, 
            Errors = response.Errors
        };
    }

    public async Task<Response> Register(RegisterData data)
    {
        var registerDto = new RegisterDTO
        {
            Email = data.Email,
            Password = data.Password,
            Username = data.Username
        };

        var request = new Request
        {
            Method = HttpMethod.Post, 
            Route = "users/register", 
            Data = registerDto
        };

        return await SendAsync(request);
    }
}
