using Domain.Users;

namespace Domain.ResetTokens;

public sealed class ResetToken
{
    public UserEmail Email { get; set; } = null!;
    public ResetTokenValue Token { get; set; } = null!;
    public ResetTokenExpiration Expiration { get; set; } = null!;
}