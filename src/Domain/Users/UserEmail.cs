using System.Text.RegularExpressions;
using SharedKernel;

namespace Domain.Users;

public sealed partial record UserEmail
{
    public string Value { get; init; }
    internal UserEmail(string value)
    {
        Value = value;
    }

    public static Result<UserEmail> Create(string value)
    {
        var isValid = IsValid(value);

        return !isValid
            ? UserErrors.EmailIsInvalid
            : new UserEmail(value);
    }

    #region Private implementation

    private static bool IsValid(string value)
    {
        return !string.IsNullOrEmpty(value) && MyRegex().IsMatch(value);
    }
    
    private const string EmailRegex = @"^(?!.*\.\.)(?!\.)[a-z0-9]+(?:[._-][a-z0-9]+)*(\+[a-z0-9-]+)?@([a-z0-9-]+\.)+[a-z]{2,4}$";

    [GeneratedRegex(EmailRegex)]
    private static partial Regex MyRegex();

    #endregion
}