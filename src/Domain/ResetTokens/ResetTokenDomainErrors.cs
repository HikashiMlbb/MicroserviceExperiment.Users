using SharedKernel;

namespace Domain.ResetTokens;

public sealed record ResetTokenDomainError(string? Code = null, string? Message = null) : Error(Code, Message);

public static class ResetTokenDomainErrors
{
    public static readonly ResetTokenDomainError ExpirationOutOfRange = new("ResetToken.ExpirationOutOfRange", "The given Reset Token Expiration is out of valid range.");
}