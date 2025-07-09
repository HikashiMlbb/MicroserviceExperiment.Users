using Application.Abstractions;
using Domain.Users;

namespace Infrastructure.Services;

public class PasswordService : IPasswordService
{
    public async Task<UserPassword> Hash(UserRawPassword rawPassword)
    {
        var hash = await Task.Run(() => BCrypt.Net.BCrypt.HashPassword(rawPassword.Value)!);
        return new UserPassword(hash);
    }

    public Task<bool> Verify(UserRawPassword passwordResultValue, UserPassword userPassword)
    {
        throw new NotImplementedException();
    }
}