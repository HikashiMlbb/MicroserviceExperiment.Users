using Application.Users;
using Domain.Users;

namespace Persistence.Users;

public class UserRepository : IUserRepository
{
    public Task<bool> IsExists(UserEmail email, UserName username)
    {
        throw new NotImplementedException();
    }

    public Task<User> Create(UserEmail email, UserName username, UserPassword password)
    {
        throw new NotImplementedException();
    }

    public Task<User?> Fetch(UserName username)
    {
        throw new NotImplementedException();
    }
}