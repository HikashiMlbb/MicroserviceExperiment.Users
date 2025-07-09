using Application.Abstractions;
using Domain.Users;

namespace Infrastructure.Services;

public class PasswordService : IPasswordService
{
    public Task<UserPassword> Hash(UserRawPassword rawPassword)
    {
        throw new NotImplementedException();
    }

    public Task<bool> Verify(UserRawPassword passwordResultValue, UserPassword userPassword)
    {
        throw new NotImplementedException();
    }
}