using System.Data;
using Npgsql;

namespace Persistence;

public class DapperConnectionFactory(string connectionString) : IDatabaseConnectionFactory
{
    public IDbConnection Create()
    {
        return new NpgsqlConnection(connectionString);
    }
}