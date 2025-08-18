using Application.Abstractions;
using Domain.Users;

namespace Infrastructure.Users;

public class PasswordService : IPasswordService
{
    public async Task<UserPassword> Hash(UserRawPassword rawPassword)
    {
        var hash = await Task.Run(() => BCrypt.Net.BCrypt.HashPassword(rawPassword.Value)!);
        return new UserPassword(hash);
    }

    public async Task<bool> Verify(UserRawPassword passwordResultValue, UserPassword userPassword)
    {
        return await Task.Run(() => BCrypt.Net.BCrypt.Verify(passwordResultValue.Value, userPassword.Value));
    }
}