namespace Core.Auth;

public interface IAuthenticationService
{
    Task<IResult> Login(LoginModel loginData);
    Task<IResult> Register(RegisterModel registerData);
    Task<IResult> ReAuthenticate(string currentRefreshToken);

    Task<IResult> ForgotPassword(ForgetPasswordModel forgotPassword);
    Task<IResult> SetNewPassword(ResetPasswordModel resetPassword);
    Task<IResult> ConfirmEmail(string Token);
}

