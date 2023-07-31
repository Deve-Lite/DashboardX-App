using Shared.Models.Users;

namespace Core.Interfaces;

public interface IUserService
{
    Task<Result<User>> GetUser();
    Task<Result> UpdateUser(User user);
    Task<Result> DeleteUser();
}
