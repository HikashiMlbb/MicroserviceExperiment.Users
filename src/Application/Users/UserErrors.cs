using SharedKernel;

namespace Application.Users;

public static class UserErrors
{
    public static Error AlreadyExists { get; } = new("User.AlreadyExists", "User with given Email or Username already exists."); 
}