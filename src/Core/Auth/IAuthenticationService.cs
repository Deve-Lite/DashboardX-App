namespace Core.Auth;

public interface IAuthenticationService
{
    Task<IResult> Login(LoginModel loginData);
    Task<IResult> Register(RegisterModel registerData);
    Task<IResult> ReAuthenticate(string currentRefreshToken);

    Task<IResult> ForgotPassword(ForgetPasswordModel forgotPassword);
    Task<IResult> SetNewPassword(ResetPasswordModel resetPassword, string token);
    Task<IResult> ConfirmEmail(string Token);
    Task<IResult> ResendConfirmEmail(ResendConfirmEmailModel model);
}

