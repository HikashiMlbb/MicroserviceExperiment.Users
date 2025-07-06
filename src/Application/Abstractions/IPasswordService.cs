using Domain.Users;

namespace Application.Abstractions;

public interface IPasswordService
{
    public Task<UserPassword> Hash(string rawPassword);
}