namespace Presentation.Auth;

public static class AuthServicesExtensions
{
    public static WebAssemblyHostBuilder AddAuthServices(this WebAssemblyHostBuilder builder)
    {
        builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();

        return builder;
    }
}
