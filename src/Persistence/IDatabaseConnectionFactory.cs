using System.Data;

namespace Persistence;

public interface IDatabaseConnectionFactory
{
    public IDbConnection Create();
}