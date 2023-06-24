using DashboardX.Tokens;
using System.Diagnostics.Contracts;

namespace DashboardX;

public interface IAuthorizationService
{
    void SaveTokens(string accessToken, string refreshToken);
    void SaveTokens(AccessToken accessToken, RefreshToken refreshToken);
    Task AuthorizeClient(HttpClient httpClient);
    Task AuthorizeMessage(HttpRequestMessage message);
    Task RefreshTokens();
    Task Logout();

    TimeSpan MaxRequestTime { get; }
    AccessToken AccessToken { get; }
    RefreshToken RefreshToken { get; }
}
