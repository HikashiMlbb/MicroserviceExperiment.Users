namespace Domain.Users;

public sealed class User
{
    public UserId Id { get; set; } = null!;
    public UserEmail Email { get; set; } = null!;
    public UserName Name { get; set; } = null!;
    public UserPassword Password { get; set; } = null!;

    public User(UserId id, UserEmail email, UserName name, UserPassword password)
    {
        Id = id;
        Email = email;
        Name = name;
        Password = password;
    }
    
    // Needed by ORM
    private User()
    {
        
    }
}