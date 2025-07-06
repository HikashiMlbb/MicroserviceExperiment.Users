namespace Domain.Users;

public class User
{
    public UserId Id { get; set; } = null!;
    public UserEmail Email { get; set; } = null!;
    public UserName Name { get; set; } = null!;
    public string Password { get; set; } = null!;
}