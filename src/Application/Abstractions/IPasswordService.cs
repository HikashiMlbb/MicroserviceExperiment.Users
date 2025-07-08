using Domain.Users;

namespace Application.Abstractions;

public interface IPasswordService
{
    public Task<UserPassword> Hash(UserRawPassword rawPassword);
    public Task<bool> Verify(UserRawPassword passwordResultValue, UserPassword userPassword);
}