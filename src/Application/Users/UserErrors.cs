using SharedKernel;

namespace Application.Users;

public static class UserErrors
{
    public static Error AlreadyExists { get; } = new("User.AlreadyExists", "User with given Email or Username already exists.");
    public static Error NotFound { get; } = new("User.NotFound", "User with given Username has not been found.");
    public static Error LoginFailed { get; } = new("User.LoginFailed", "The entered password is incorrect.");
}