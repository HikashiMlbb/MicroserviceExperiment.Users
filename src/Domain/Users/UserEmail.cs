using System.Text.RegularExpressions;
using SharedKernel;

namespace Domain.Users;

public sealed partial record UserEmail
{
    public string Value { get; init; }
    private UserEmail(string value)
    {
        Value = value;
    }

    public static Result<UserEmail> Create(string value)
    {
        var isMatch = MyRegex().IsMatch(value);

        return !isMatch
            ? UserErrors.EmailIsInvalid
            : new UserEmail(value);
    }

    #region Private implementation

    private const string EmailRegex = @"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$";

    [GeneratedRegex(EmailRegex)]
    private static partial Regex MyRegex();

    #endregion
}