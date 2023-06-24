using DashboardX.Tokens;
using System.Diagnostics.Contracts;

namespace DashboardX.Services.Interfaces;

public interface IAuthorizationService
{
    void AuthenticateSession(string accessToken, string refreshToken);
    void AuthenticateSession(AccessToken accessToken, RefreshToken refreshToken);
    Task SaveTokensInStorage();
    Task RemoveTokensFromStorage();
    Task AuthorizeClient(HttpClient httpClient);
    Task AuthorizeMessage(HttpRequestMessage message);
    Task RefreshTokens();
    Task Logout();

    TimeSpan MaxRequestTime { get; }
    AccessToken AccessToken { get; }
    RefreshToken RefreshToken { get; }
}
