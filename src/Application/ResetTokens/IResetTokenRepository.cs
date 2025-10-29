using Domain.ResetTokens;
using Domain.Users;

namespace Application.ResetTokens;

public interface IResetTokenRepository
{
    public Task<UserEmail?> Find(ResetTokenValue tokenValue);
    public Task Save(ResetToken token);
    public Task<bool> IsRequested(UserEmail? emailResultValue);
}