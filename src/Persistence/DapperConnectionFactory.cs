using System.Data;

namespace Persistence;

public class DapperConnectionFactory : IDatabaseConnectionFactory
{
    public IDbConnection Create()
    {
        throw new NotImplementedException();
    }
}