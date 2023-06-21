

using System.ComponentModel.DataAnnotations;

namespace DashboardXModels.Auth.DTO;

public class RegisterDTO
{
    [Required, MinLength(3), MaxLength(30)]
    public string Username { get; set; } = string.Empty;
    [Required, MinLength(6), MaxLength(30)]
    public string Password { get; set; } = string.Empty;
    [Required, MinLength(6), MaxLength(256), EmailAddress]
    public string Email { get; set; } = string.Empty;
}
