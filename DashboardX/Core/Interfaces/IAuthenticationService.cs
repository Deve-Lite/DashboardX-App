namespace Core.Interfaces;

public interface IAuthenticationService
{
    Task<IResult> Login(LoginModel loginData);
    Task<IResult> Register(RegisterModel registerData);
    Task<IResult> ReAuthenticate(string currentRefreshToken);
}

