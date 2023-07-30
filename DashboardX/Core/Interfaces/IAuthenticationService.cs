namespace Core.Interfaces;

public interface IAuthenticationService
{
    Task<Result> Login(LoginData loginData);
    Task<Result> Register(RegisterData registerData);
}

