
namespace DashboardXModels;

public class User
{
    public string UserId { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public bool IsAdmin { get; set; }

    public AppTheme AppTheme { get; set; } = AppTheme.Inherit;
    public string Language { get; set; } = string.Empty;
}
