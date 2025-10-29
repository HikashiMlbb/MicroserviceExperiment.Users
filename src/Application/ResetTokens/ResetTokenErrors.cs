using SharedKernel;

namespace Application.ResetTokens;

public record ResetTokenError(string? Code = null, string? Message = null) : Error(Code, Message);

public static class ResetTokenErrors
{
    public static readonly ResetTokenError ValidationError = new("ResetToken.Invalid", "Given Reset Token Value is invalid.");
    public static readonly ResetTokenError AlreadyRequested = new("ResetToken.AlreadyRequested", "Reset token for this account already requested.");
    public static readonly ResetTokenError Empty = new("ResetToken.Empty", "Given reset token is empty.");
    public static readonly ResetTokenError NotExistsOrExpired = new("ResetToken.NotExistsOrExpired", "Given reset token is not exists or already expired.");

}