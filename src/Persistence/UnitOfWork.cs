using System.Data;
using Application.Abstractions;
using Application.ResetTokens;
using Application.Users;
using Microsoft.Extensions.Caching.Distributed;
using Persistence.ResetTokens;
using Persistence.Users;

namespace Persistence;

public class UnitOfWork : IUnitOfWork
{
    private bool _isDisposed;
    private IUserRepository? _users;
    private IResetTokenRepository? _resetTokens;
    public IUserRepository Users => _users ??= new UserRepository(Connection, Transaction);

    public IResetTokenRepository ResetTokens => _resetTokens ??= new ResetTokenRepository(_cache);

    public IDbConnection Connection { get; }
    public IDbTransaction Transaction { get; }
    private readonly IDistributedCache _cache;

    public UnitOfWork(IDatabaseConnectionFactory factory, IDistributedCache cache)
    {
        Connection = factory.Create();
        Connection.Open();
        Transaction = Connection.BeginTransaction();
        _cache = cache;
    }

    public Task Commit()
    {
        if (_isDisposed) throw new ObjectDisposedException(nameof(UnitOfWork), "Cannot commit disposed context.");
        Transaction.Commit();

        return Task.CompletedTask;
    }

    public Task Rollback()
    {
        if (_isDisposed) throw new ObjectDisposedException(nameof(UnitOfWork), "Cannot rollback disposed context.");
        Transaction.Rollback();
        
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool _)
    {
        if (_isDisposed) return;
        
        Transaction.Dispose();
        Connection.Close();
        Connection.Dispose();
        _isDisposed = true;
    }
}