using SharedKernel;

namespace Domain.Users;

public static class UserErrors
{
    public static Error EmailIsInvalid { get; } = new("Email.Invalid", "The given email has invalid format.");
    public static Error UsernameIsInvalid { get; } = new("Username.Invalid", "The given username has invalid format.");
    public static Error PasswordIsOutOfRange { get; } = new("Password.OutOfRange", "The given password is too long or too short.");
}