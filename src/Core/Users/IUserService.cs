﻿namespace Core.Users;

public interface IUserService
{
    Task<IResult<User>> GetUser();
    Task<IResult> UpdatePreferences(Preferences dto);
    Task<IResult> ChangePassword(ChangePasswordModel dto);
    Task<IResult> RemoveAccount(PasswordConfirm dto);
}
