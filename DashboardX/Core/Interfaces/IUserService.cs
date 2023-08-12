using Shared.Models.Users;

namespace Core.Interfaces;

public interface IUserService
{
    Task<IResult<User>> GetUser();
    Task<IResult> UpdateUser(User user);
    Task<IResult> DeleteUser();
}
