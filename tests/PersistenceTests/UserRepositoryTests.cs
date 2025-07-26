using Dapper;
using Domain.Users;
using Npgsql;
using Persistence;
using Testcontainers.PostgreSql;

namespace PersistenceTests;

[TestFixture]
[Parallelizable(ParallelScope.Children)]
public class UserRepositoryTests
{
    private PostgreSqlContainer _container = null!;
    private DapperConnectionFactory _factory = null!;
    
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

    [Test]
    public async Task IsExists_NotFound_ReturnsFalse()
    {
        // Arrange
        using var scope = await TestTransactionScope.Create(_factory);
        var uow = scope.Uow;
        var email = new UserEmail("someemail@mail.com");
        var username = new UserName("SomeUser");
        
        // Act
        var result = await uow.Users.IsExists(email, username);

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public async Task IsExists_NotFound_AnotherRecord_ReturnsFalse()
    {
        // Arrange
        using var scope = await TestTransactionScope.Create(_factory);
        var uow = scope.Uow;
        var connection = scope.Connection;
        var transaction = scope.Transaction;
        
        await connection.ExecuteAsync("INSERT INTO \"Users\" VALUES (DEFAULT, 'someemail@mail.ru', 'SomeUsername', 'password')", transaction: transaction);
        var email = new UserEmail("someemail@mail.com");
        var username = new UserName("SomeUser");
        
        // Act
        var result = await uow.Users.IsExists(email, username);

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public async Task IsExists_FoundByEmail_ReturnsTrue()
    {
        // Arrange
        using var scope = await TestTransactionScope.Create(_factory);
        var uow = scope.Uow;
        var connection = scope.Connection;
        var transaction = scope.Transaction;
        
        await connection.ExecuteAsync("INSERT INTO \"Users\" VALUES (DEFAULT, 'someemail@mail.com', 'SomeUsername', 'password')", transaction: transaction);
        var email = new UserEmail("someemail@mail.com");
        var username = new UserName("SomeUser");
        
        // Act
        var result = await uow.Users.IsExists(email, username);

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public async Task IsExists_FoundByUsername_ReturnsTrue()
    {
        // Arrange
        using var scope = await TestTransactionScope.Create(_factory);
        var uow = scope.Uow;
        var connection = scope.Connection;
        var transaction = scope.Transaction;
        
        await connection.ExecuteAsync("INSERT INTO \"Users\" VALUES (DEFAULT, 'someemail@mail.ru', 'SomeUser', 'password')", transaction: transaction);
        var email = new UserEmail("someemail@mail.com");
        var username = new UserName("SomeUser");
        
        // Act
        var result = await uow.Users.IsExists(email, username);

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public async Task Create_Successfully_ReturnsSuccess()
    {
        // Arrange
        using var scope = await TestTransactionScope.Create(_factory);
        var uow = scope.Uow;
        
        var email = new UserEmail("someemail@mail.com");
        var username = new UserName("SomeUser");
        var password = new UserPassword("SomePassword");
        
        // Act
        var result = await uow.Users.Create(email, username, password);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Email, Is.EqualTo(email));
        Assert.That(result.Name, Is.EqualTo(username));
        Assert.That(result.Password, Is.EqualTo(password));
    }
    
    [Test]
    public async Task Create_Twice_ReturnsException()
    {
        using var scope = await TestTransactionScope.Create(_factory);
        var uow = scope.Uow;
        
        var email = new UserEmail("someemail@mail.com");
        var username = new UserName("SomeUser");
        var password = new UserPassword("SomePassword");
        
        // Act
        var result1 = await uow.Users.Create(email, username, password);
        var result2 = async () => await uow.Users.Create(email, username, password);

        // Assert
        Assert.That(result1, Is.Not.Null);
        Assert.That(result1.Email, Is.EqualTo(email));
        Assert.That(result1.Name, Is.EqualTo(username));
        Assert.That(result1.Password, Is.EqualTo(password));
        Assert.ThrowsAsync<PostgresException>(async () => await result2());
    }

    [Test]
    public async Task Fetch_NotExists_ReturnsNull()
    {
        // Arrange
        using var scope = await TestTransactionScope.Create(_factory);
        var uow = scope.Uow;
        
        var username = new UserName("Vasily");
        
        // Act
        var result = await uow.Users.Fetch(username);

        // Assert
        Assert.That(result, Is.Null);
    }
    
    [Test]
    public async Task Fetch_Exists_ReturnsUser()
    {
        // Arrange
        using var scope = await TestTransactionScope.Create(_factory);
        var uow = scope.Uow;
        var connection = scope.Connection;
        var transaction = scope.Transaction;
        
        var email = new UserEmail("vasily@mail.com");
        var username = new UserName("Vasily");
        var password = new UserPassword("S0m3-h@53d-p@55w0rd");
        const string sql = "INSERT INTO \"Users\" VALUES (DEFAULT, @Email, @Username, @Password);";
        var @params = new { Email = email.Value, Username = username.Value, Password = password.Value };
        await connection.ExecuteAsync(sql, @params, transaction);
        
        // Act
        var result = await uow.Users.Fetch(username);

        // Assert
        Assert.That(result, Is.Not.Null);
    }
}