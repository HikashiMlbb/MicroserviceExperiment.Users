namespace Domain.Users;

public sealed class User(UserId id, UserEmail email, UserName name, UserPassword password)
{
    public UserId Id { get; set; } = id;
    public UserEmail Email { get; set; } = email;
    public UserName Name { get; set; } = name;
    public UserPassword Password { get; set; } = password;
}