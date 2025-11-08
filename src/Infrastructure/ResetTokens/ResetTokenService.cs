using Application.Abstractions;
using Domain.ResetTokens;

namespace Infrastructure.ResetTokens;

public class ResetTokenService(ResetTokenSettings settings) : IResetTokenService
{
    public Task<ResetTokenValue> Generate()
    {
        return Task.FromResult(new ResetTokenValue(Guid.NewGuid().ToString()));
    }

    public Task<ResetTokenExpiration> GetExpiration()
    {
        var tokenExpiration = ResetTokenExpiration.Create(DateTime.UtcNow + settings.Expiration);
        return !tokenExpiration.IsSuccess 
            ? throw new ArgumentException("Token Expiration Span is not valid.") 
            : Task.FromResult(tokenExpiration.Value!);
    }
}