using SharedKernel;

namespace Domain.Users;

public static class UserErrors
{
    public static Error EmailIsInvalid { get; } = new("Email.Invalid", "The given email has invalid format.");
}