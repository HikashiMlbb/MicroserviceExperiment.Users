using System.Data;
using Dapper;
using Domain.Users;
using Npgsql;
using Persistence;
using Persistence.Users;
using Testcontainers.PostgreSql;

namespace PersistenceTests;

[TestFixture]
public class UserRepositoryTests
{
    private PostgreSqlContainer _container = null!;
    private DapperConnectionFactory _factory = null!;
    private IDbConnection _connection = null!;
    private UserRepository _repo = null!;
    
    [OneTimeSetUp]
    public async Task SetupOnce()
    {
        _container = new PostgreSqlBuilder().Build();
        await _container.StartAsync();
        _factory = new DapperConnectionFactory(_container.GetConnectionString());
        _repo = new UserRepository(_factory);

        using var connection = _factory.Create();
        const string sql = """
                           CREATE TABLE IF NOT EXISTS "Users" (
                               "Id" SERIAL PRIMARY KEY,
                               "Email" VARCHAR UNIQUE,
                               "Username" VARCHAR UNIQUE,
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

    [Test]
    public async Task IsExists_NotFound_ReturnsFalse()
    {
        // Arrange
        var email = new UserEmail("someemail@mail.com");
        var username = new UserName("SomeUser");
        
        // Act
        var result = await _repo.IsExists(email, username);

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public async Task IsExists_NotFound_AnotherRecord_ReturnsFalse()
    {
        // Arrange
        await _connection.ExecuteAsync("INSERT INTO \"Users\" VALUES (DEFAULT, 'someemail@mail.ru', 'SomeUsername', 'password')");
        var email = new UserEmail("someemail@mail.com");
        var username = new UserName("SomeUser");
        
        // Act
        var result = await _repo.IsExists(email, username);

        // Assert
        Assert.That(result, Is.False);
        
        // Clean
        await _connection.ExecuteAsync("DELETE FROM \"Users\";");
    }

    [Test]
    public async Task IsExists_FoundByEmail_ReturnsTrue()
    {
        // Arrange
        await _connection.ExecuteAsync("INSERT INTO \"Users\" VALUES (DEFAULT, 'someemail@mail.com', 'SomeUsername', 'password')");
        var email = new UserEmail("someemail@mail.com");
        var username = new UserName("SomeUser");
        
        // Act
        var result = await _repo.IsExists(email, username);

        // Assert
        Assert.That(result, Is.True);
        
        // Clean
        await _connection.ExecuteAsync("DELETE FROM \"Users\";");
    }

    [Test]
    public async Task IsExists_FoundByUsername_ReturnsTrue()
    {
        // Arrange
        await _connection.ExecuteAsync("INSERT INTO \"Users\" VALUES (DEFAULT, 'someemail@mail.ru', 'SomeUser', 'password')");
        var email = new UserEmail("someemail@mail.com");
        var username = new UserName("SomeUser");
        
        // Act
        var result = await _repo.IsExists(email, username);

        // Assert
        Assert.That(result, Is.True);
        
        // Clean
        await _connection.ExecuteAsync("DELETE FROM \"Users\";");
    }

    [Test]
    public async Task Create_Successfully_ReturnsSuccess()
    {
        var email = new UserEmail("someemail@mail.com");
        var username = new UserName("SomeUser");
        var password = new UserPassword("SomePassword");
        
        // Act
        var result = await _repo.Create(email, username, password);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Email, Is.EqualTo(email));
        Assert.That(result.Name, Is.EqualTo(username));
        Assert.That(result.Password, Is.EqualTo(password));
        
        // Clean
        await _connection.ExecuteAsync("DELETE FROM \"Users\";");
    }
    
    [Test]
    public async Task Create_Twice_ReturnsException()
    {
        var email = new UserEmail("someemail@mail.com");
        var username = new UserName("SomeUser");
        var password = new UserPassword("SomePassword");
        
        // Act
        var result1 = await _repo.Create(email, username, password);
        var result2 = async () => await _repo.Create(email, username, password);

        // Assert
        Assert.That(result1, Is.Not.Null);
        Assert.That(result1.Email, Is.EqualTo(email));
        Assert.That(result1.Name, Is.EqualTo(username));
        Assert.That(result1.Password, Is.EqualTo(password));
        Assert.ThrowsAsync<PostgresException>(async () => await result2());
        
        // Clean
        await _connection.ExecuteAsync("DELETE FROM \"Users\";");
    }

    [Test]
    public async Task Fetch_NotExists_ReturnsNull()
    {
        // Arrange
        var username = new UserName("Vasily");
        
        // Act
        var result = await _repo.Fetch(username);

        // Assert
        Assert.That(result, Is.Null);
    }
    
    [Test]
    public async Task Fetch_Exists_ReturnsUser()
    {
        // Arrange
        var email = new UserEmail("vasily@mail.com");
        var username = new UserName("Vasily");
        var password = new UserPassword("S0m3-h@53d-p@55w0rd");
        const string sql = "INSERT INTO \"Users\" VALUES (DEFAULT, @Email, @Username, @Password);";
        var @params = new { Email = email.Value, Username = username.Value, Password = password.Value };
        await _connection.ExecuteAsync(sql, @params);
        
        // Act
        var result = await _repo.Fetch(username);

        // Assert
        Assert.That(result, Is.Not.Null);
        
        // Clean
        await _connection.ExecuteAsync("DELETE FROM \"Users\";");
    }
}