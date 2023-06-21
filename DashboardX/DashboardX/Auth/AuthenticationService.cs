using DashboardX.Auth.Services;
using DashboardXModels.Auth.DTO;

namespace DashboardX.Auth;

public class AuthenticationService : BaseService, IAuthenticationService
{
    public AuthenticationService(HttpClient httpClient, IAuthorizationService authorizationService) : base(httpClient, authorizationService)
    {
    }

    public async Task<Response> Login(string email, string password)
    {
        var data = new LoginDTO
        {
            Email = email,
            Password = password
        };

        var request = new Request
        {
            Method = HttpMethod.Post,
            Route = "api/auth/login",
            Data = data
        };

        var response = await SendAsync<TokenDTO>(request);

        if (response.Success)
        {
            authorizationService.SaveTokens(response.Data.AccessToken, response.Data.RefreshToken);

            return new Response 
            { 
                StatusCode = response.StatusCode
            };
        }

        return new Response 
        { 
            StatusCode = response.StatusCode, 
            Errors = response.Errors
        };
    }

    public async Task<Response> Register(string username, string email, string password)
    {
        var data = new RegisterDTO
        {
            Username = username,
            Email = email,
            Password = password
        };

        var request = new Request
        {
            Method = HttpMethod.Post, 
            Route = "api/auth/register", 
            Data = data 
        };

        return await SendAsync(request);
    }
}
