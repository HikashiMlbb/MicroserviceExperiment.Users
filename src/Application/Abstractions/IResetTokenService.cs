using Domain.ResetTokens;

namespace Application.Abstractions;

public interface IResetTokenService
{
    public Task<ResetTokenValue> Generate();
    public Task<ResetTokenExpiration> GetExpiration();
}