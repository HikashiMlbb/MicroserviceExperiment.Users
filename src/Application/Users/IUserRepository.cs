using Domain.Users;

namespace Application.Users;

public interface IUserRepository
{
    public Task<bool> IsExists(UserEmail? email = null, UserName? username = null);
    public Task<User> Create(UserEmail email, UserName username, UserPassword password);
    public Task<User?> Fetch(UserName username);
    public Task ChangePassword(UserEmail email, UserPassword password);
}