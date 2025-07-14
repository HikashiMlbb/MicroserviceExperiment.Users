using System.Data;
using Dapper;
using Persistence;
using Testcontainers.PostgreSql;

namespace PersistenceTests;

[TestFixture]
public class UserRepositoryTests
{
    private PostgreSqlContainer _container = null!;
    private DapperConnectionFactory _factory = null!;
    private IDbConnection _connection = null!;
    
    [OneTimeSetUp]
    public async Task SetupOnce()
    {
        _container = new PostgreSqlBuilder().Build();
        await _container.StartAsync();
        _factory = new DapperConnectionFactory(_container.GetConnectionString());

        using var connection = _factory.Create();
        const string sql = """
                           CREATE TABLE IF NOT EXISTS "Users" (
                               "Id" SERIAL PRIMARY KEY,
                               "Email" VARCHAR,
                               "Username" VARCHAR,
                               "Password" VARCHAR
                           );
                           """;
        await connection.ExecuteAsync(sql);
    }

    [OneTimeTearDown]
    public async Task TearDownOnce()
    {
        await _container.StopAsync();
        await _container.DisposeAsync();
    }

    [SetUp]
    public void Setup()
    {
        _connection = _factory.Create();
    }

    [TearDown]
    public void TearDown()
    {
        _connection.Close();
        _connection.Dispose();
    }
}