using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;
using Migration.Migrations;

namespace Migration;

public static class DatabaseMigrator
{
    public static void MigrateDatabase(string connectionString)
    {
        var provider = new ServiceCollection()
            .AddFluentMigratorCore()
            .ConfigureRunner(rb => rb
                .AddPostgres()
                .WithGlobalConnectionString(connectionString)
                .ScanIn(typeof(Initial).Assembly).For.Migrations())
            .AddLogging(lb => lb.AddFluentMigratorConsole())
            .BuildServiceProvider();

        var runner = provider.GetRequiredService<IMigrationRunner>();
        runner.MigrateUp();
    }
}