namespace Core.Interfaces;

public interface IAuthenticationService
{
    Task<Result> Login(LoginRequest loginData);
    Task<Result> Register(RegisterRequest registerData);
}

