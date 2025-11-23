using System.Data;
using Application.Users;
using Dapper;
using Domain.Users;

namespace Persistence.Users;

public class UserRepository(IDbConnection connection, IDbTransaction transaction) : IUserRepository
{
    public async Task<bool> IsExists(UserEmail? email = null, UserName? username = null)
    {
        const string sql = "SELECT 1 FROM \"Users\" WHERE \"Email\" = @Email OR \"Username\" = @Username LIMIT 1;";
        return await connection.QueryFirstOrDefaultAsync<bool>(sql, new { Email = email.Value, Username = username.Value }, transaction);
    }

    public async Task<User> Create(UserEmail email, UserName username, UserPassword password)
    {
        const string sql = """
                           INSERT INTO "Users" ("Email", "Username", "Password")
                           VALUES (@Email, @Username, @Password)
                           RETURNING "Id";
                           """;
        var id = await connection.QueryFirstAsync<int>(sql, new { Email = email.Value, Username = username.Value, Password = password.Value }, transaction);
        return new User(new UserId(id), email, username, password);
    }

    public async Task<User?> Fetch(UserName username)
    {
        const string sql = """
                           SELECT
                                "Id",
                                "Email",
                                "Username" AS "Name",
                                "Password"
                           FROM "Users"
                           WHERE "Username" = @Username
                           LIMIT 1;
                           """;
        var raw = await connection.QueryFirstOrDefaultAsync<RawUser>(sql, new { Username = username.Value }, transaction);
        return raw is null
            ? null
            : new User(
                id: new UserId(raw.Id),
                email: new UserEmail(raw.Email),
                name: new UserName(raw.Name),
                password: new UserPassword(raw.Password)
            );
    }

    public async Task ChangePassword(UserEmail email, UserPassword password)
    {
        const string sql = """
                           UPDATE "Users"
                           SET "Password" = @Password
                           WHERE "Email" = @Email
                           """;
        await connection.ExecuteAsync(sql, new { Password = password.Value, Email = email.Value }, transaction);
    }
}