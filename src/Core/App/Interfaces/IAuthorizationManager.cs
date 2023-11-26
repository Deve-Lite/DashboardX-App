namespace Core.App.Interfaces;

public interface IAuthorizationManager
{
    Task ExtendSession(string accessToken, string refreshToken);
    string GetRefreshToken();
    void ObserveLogout(ILogoutObserver logoutObserver);
    Task Logout();
}
