using SharedKernel;

namespace Domain.Users;

public sealed record UserName
{
    public string Value { get; init; }

    private UserName(string value)
    {
        Value = value;
    }

    public static Result<UserName> Create(string value)
    {
        return !IsValid(value) 
            ? UserErrors.UsernameIsInvalid 
            : new UserName(value);
    }

    #region Private implementation

    private static bool IsValid(string value)
    {
        return value.Length is >= 4 and <= 20 && value.All(char.IsAsciiLetterOrDigit);
    }

    #endregion
}