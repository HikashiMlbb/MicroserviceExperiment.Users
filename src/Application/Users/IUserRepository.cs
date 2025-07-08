using Domain.Users;

namespace Application.Users;

public interface IUserRepository
{
    public Task<bool> IsExists(UserEmail email, UserName username);
    public Task<User> Create(UserEmail email, UserName username, UserPassword password);
}