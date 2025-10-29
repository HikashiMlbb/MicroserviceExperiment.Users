using Domain.Users;

namespace Domain.ResetTokens;

public sealed class ResetToken
{
    public UserEmail Email { get; set; } = null!;
    public ResetTokenValue Token { get; set; } = null!;
    public ResetTokenExpiration Expiration { get; set; } = null!;

    public ResetToken(UserEmail email, ResetTokenValue value, ResetTokenExpiration expiration)
    {
        Email = email;
        Token = value;
        Expiration = expiration;
    }
}