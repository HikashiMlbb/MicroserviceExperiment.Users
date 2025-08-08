using FluentMigrator;
using FluentMigrator.Postgres;

namespace Migration.Migrations;

[Migration(202508081813)]
public class UsersTable : FluentMigrator.Migration
{
    public override void Up()
    {
        Create.Table("Users")
            .WithColumn("Id").AsInt32().NotNullable().PrimaryKey().Identity()
            .WithColumn("Email").AsString().NotNullable().Unique()
            .WithColumn("Username").AsString().NotNullable().Unique()
            .WithColumn("Password").AsString().NotNullable();
    }

    public override void Down()
    {
        Delete.Table("Users");
    }
}