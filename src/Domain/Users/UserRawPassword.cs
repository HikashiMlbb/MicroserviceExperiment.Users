using SharedKernel;

namespace Domain.Users;

public record UserRawPassword
{
    public string Value { get; init; }
    
    internal UserRawPassword(string value)
    {
        Value = value;
    }

    public static Result<UserRawPassword> Create(string value)
    {
        return !IsValid(value)
            ? UserDomainErrors.PasswordIsOutOfRange
            : new UserRawPassword(value);
    }

    private static bool IsValid(string value)
    {
        return !string.IsNullOrWhiteSpace(value) && value.Length is > 4 and < 20;
    }
}