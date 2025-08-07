using SharedKernel;

namespace Domain.Users;

public record UserDomainError(string? Code = null, string? Message = null) : Error(Code, Message);

public static class UserDomainErrors
{
    public static readonly UserDomainError EmailIsInvalid = new("Email.Invalid", "The given email has invalid format.");
    public static readonly UserDomainError UsernameIsInvalid = new("Username.Invalid", "The given username has invalid format.");
    public static readonly UserDomainError PasswordIsOutOfRange = new("Password.OutOfRange", "The given password is too long or too short.");
}