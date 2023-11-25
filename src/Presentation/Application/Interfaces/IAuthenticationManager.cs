﻿namespace Presentation.Application.Interfaces;

public interface IAuthenticationManager
{
    Task ExtendSession(string accessToken, string refreshToken);
    string GetRefreshToken();
    void ObserveLogout(ILogoutObserver logoutObserver);
    Task Logout();
}