using System.Data;
using Application.Users;
using Persistence.Users;

namespace Persistence;

public class UnitOfWork : IUnitOfWork
{
    private bool _isDisposed;
    private IUserRepository? _users;
    public IUserRepository Users => _users ??= new UserRepository(Connection, Transaction);

    public IDbConnection Connection { get; }
    public IDbTransaction Transaction { get; }

    public UnitOfWork(IDatabaseConnectionFactory factory)
    {
        Connection = factory.Create();
        Connection.Open();
        Transaction = Connection.BeginTransaction();
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