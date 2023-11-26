using System.Net.Http.Headers;

namespace Presentation.Auth;

public class AuthenticationService : BaseService, IAuthenticationService
{
    private readonly ApplicationStateProvider _applicationStateProvider;

    public AuthenticationService(HttpClient httpClient,
        ILogger<AuthenticationService> logger,
        AuthenticationStateProvider authenticationStateProvider) : base(httpClient, logger)
    {
        _applicationStateProvider = (ApplicationStateProvider?)authenticationStateProvider!;
    }

    public async Task<IResult> Login(LoginModel data)
    {
        var request = new Request<LoginModel>
        {
            Method = HttpMethod.Post,
            Route = "api/v1/users/login",
            Data = data
        };

        var result = await SendAsync<Tokens, LoginModel>(request);

        if (result.Succeeded)
        {
            var accessToken = result.Data!.AccessToken;
            var refreshToken = result.Data!.RefreshToken;

            await _applicationStateProvider.Login(accessToken, refreshToken);
        }

        return result;
    }

    public async Task<IResult> Register(RegisterModel data)
    {
        var request = new Request<RegisterModel>
        {
            Method = HttpMethod.Post, 
            Route = "api/v1/users/register", 
            Data = data
        };

        return await SendAsync(request);
    }

    public async Task<IResult> ReAuthenticate(string currentRefreshToken)
    {
        var request = new Request
        {
            Method = HttpMethod.Post,
            Route = "api/v1/users/me/tokens"
        };

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", currentRefreshToken);

        var response = await SendAsync<Tokens>(request);

        if (response.Succeeded)
        {
            var accessToken = response.Data!.AccessToken;
            var refreshToken = response.Data!.RefreshToken;
            await _applicationStateProvider.Login(accessToken, refreshToken);

            return Result.Success(response.StatusCode);
        }

        await _applicationStateProvider.Logout();

        return Result.Fail(response.StatusCode);
    }

    public async Task<IResult> ForgotPassword(ForgetPasswordModel forgotPassword)
    {
        var request = new Request<ForgetPasswordModel>
        {
            Method = HttpMethod.Post,
            Route = "api/v1/users/reset-password",
            Data = forgotPassword
        };

        var response = await SendAsync(request);

        return response;
    }
    
    public async Task<IResult> ResetPassword(ResetPasswordModel resetPassword)
    {
        var request = new Request<ResetPasswordModel>
        {
            Method = HttpMethod.Patch,
            Route = "api/v1/users/reset-password",
            Data = resetPassword
        };

        var response = await SendAsync(request);

        return response;
    }

    public async Task<IResult> SetNewPassword(ResetPasswordModel resetPassword, string token)
    {
        var request = new Request<ResetPasswordModel>
        {
            Method = HttpMethod.Patch,
            Route = "api/v1/users/reset-password",
            Data = resetPassword
        };

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await SendAsync(request);

        return response;
    }

    public async Task<IResult> ConfirmEmail(string token)
    {
        var request = new Request
        {
            Method = HttpMethod.Post,
            Route = "api/v1/users/confirm-account"
        };

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await SendAsync(request);

        return response;
    }

    public async Task<IResult> ResendConfirmEmail(ResendConfirmEmailModel model)
    {
        var request = new Request<ResendConfirmEmailModel>
        {
            Method = HttpMethod.Post,
            Route = "api/v1/users/confirm-account/resend",
            Data = model
        };

        var response = await SendAsync(request);

        return response;
    }
}
