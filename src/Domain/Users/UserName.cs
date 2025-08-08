using SharedKernel;

namespace Domain.Users;

public sealed record UserName
{
    public string Value { get; init; }

    internal UserName(string value)
    {
        Value = value;
    }

    public static Result<UserName> Create(string value)
    {
        return !IsValid(value) 
            ? UserDomainErrors.UsernameIsInvalid 
            : new UserName(value);
    }

    #region Private implementation

    private static bool IsValid(string value)
    {
        return !string.IsNullOrEmpty(value) && value.Length is >= 4 and <= 20 && value.All(char.IsAsciiLetterOrDigit);
    }

    #endregion
}