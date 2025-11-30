using Application.ResetTokens;
using Application.Users;

namespace Application.Abstractions;

public interface IUnitOfWork : IDisposable
{
    public IUserRepository Users { get; }
    public IResetTokenRepository ResetTokens { get; }
    
    public Task Commit();
    public Task Rollback();
}