using SharedKernel;

namespace Domain.Users;

public record UserRawPassword
{
    public string Value { get; init; }
    
    private UserRawPassword(string value)
    {
        Value = value;
    }

    public static Result<UserRawPassword> Create(string value)
    {
        return !IsValid(value)
            ? UserErrors.PasswordIsOutOfRange
            : new UserRawPassword(value);
    }

    private static bool IsValid(string value)
    {
        return !string.IsNullOrWhiteSpace(value) && value.Length is > 4 and < 20;
    }
}