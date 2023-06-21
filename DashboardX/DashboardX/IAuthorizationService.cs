using System.Diagnostics.Contracts;

namespace DashboardX.Auth.Services;

public interface IAuthorizationService
{
    void SaveTokens(string accessToken, string refreshToken);
    Task AuthorizeClient(HttpClient httpClient);
    Task AuthorizeMessage(HttpRequestMessage message);
    Task RefreshTokens();
    Task Logout();
    bool IsAuthenticated { get; }
}
