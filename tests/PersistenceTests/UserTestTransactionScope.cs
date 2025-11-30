using System.Data;
using Persistence;

namespace PersistenceTests;

public class UserTestTransactionScope : IDisposable
{
    public UnitOfWork Uow { get; }

    public IDbConnection Connection { get; }
    public IDbTransaction Transaction { get; }

    private UserTestTransactionScope(UnitOfWork uow, IDbConnection connection, IDbTransaction transaction)
    {
        Uow = uow;
        Connection = connection;
        Transaction = transaction;
    }
    
    public static Task<UserTestTransactionScope> Create(IDatabaseConnectionFactory factory)
    {
        var uow = new UnitOfWork(factory, null);
        var connection = uow.Connection;
        var transaction = uow.Transaction;
        return Task.FromResult(new UserTestTransactionScope(uow, connection, transaction));
    }
    
    public void Dispose()
    {
        Transaction.Rollback();
        Transaction.Dispose();
        Connection.Close();
        Connection.Dispose();
        Uow.Dispose();
    }
}