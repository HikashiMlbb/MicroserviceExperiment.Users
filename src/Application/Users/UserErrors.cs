using SharedKernel;

namespace Application.Users;

public record UserApplicationError(string? Code = null, string? Message = null) : Error(Code, Message);

public static class UserApplicationErrors
{
     public static UserApplicationError AlreadyExists { get; } = new("User.AlreadyExists", "User with given Email or Username already exists.");
     public static UserApplicationError NotFound { get; } = new("User.NotFound", "User with given Username has not been found.");
     public static UserApplicationError LoginFailed { get; } = new("User.LoginFailed", "The entered password is incorrect.");
}