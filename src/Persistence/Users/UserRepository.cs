using Application.Users;
using Dapper;
using Domain.Users;

namespace Persistence.Users;

public class UserRepository(IDatabaseConnectionFactory factory) : IUserRepository
{
    public async Task<bool> IsExists(UserEmail email, UserName username)
    {
        using var db = factory.Create();
        const string sql = "SELECT 1 FROM \"Users\" WHERE \"Email\" = @Email OR \"Username\" = @Username LIMIT 1;";
        return await db.QueryFirstOrDefaultAsync<bool>(sql, new { Email = email.Value, Username = username.Value });
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