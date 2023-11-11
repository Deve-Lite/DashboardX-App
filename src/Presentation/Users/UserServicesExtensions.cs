namespace Presentation.Users;

public static class UserServicesExtensions
{
    public static WebAssemblyHostBuilder AddUserServices(this WebAssemblyHostBuilder builder)
    {
        builder.Services.AddScoped<IUserService, UserService>();

        builder.Services.AddSingleton<IPrefrenceService, PreferenceService>();

        return builder;
    }
}
