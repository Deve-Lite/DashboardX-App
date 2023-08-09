namespace Core.Interfaces;

public interface IAuthenticationService
{
    Task<IResult> Login(LoginRequest loginData);
    Task<IResult> Register(RegisterRequest registerData);
}

