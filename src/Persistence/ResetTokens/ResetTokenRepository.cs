using Application.ResetTokens;
using Domain.ResetTokens;
using Domain.Users;
using Microsoft.Extensions.Caching.Distributed;

namespace Persistence.ResetTokens;

public class ResetTokenRepository(IDistributedCache cache) : IResetTokenRepository
{
    public async Task<UserEmail?> Find(ResetTokenValue tokenValue)
    {
        var found = await cache.GetStringAsync(tokenValue.Value);
        return found is null ? null : new UserEmail(found);
    }

    public async Task Save(ResetToken resetToken)
    {
        var token = resetToken.Token.Value;
        var email = resetToken.Email.Value;
        var expiration = resetToken.Expiration.Value;
        
        var options = new DistributedCacheEntryOptions { AbsoluteExpiration = expiration };
        
        await cache.SetStringAsync(token, email, options);
        await cache.SetStringAsync(email, string.Empty, options);
    }

    public async Task<bool> IsRequested(UserEmail emailResultValue)
    {
        var found = await cache.GetStringAsync(emailResultValue.Value);
        return found is not null;
    }
}