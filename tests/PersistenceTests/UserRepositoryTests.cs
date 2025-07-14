using System.Data;
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