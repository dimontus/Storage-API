using FluentMigrator;

namespace Api.DataAccess.Migrations
{
    [Migration(20200304175200)]
    public class AddStorageTable : Migration
    {
        public override void Up()
        {
            Create.Table("Storage")
                .WithEntityBaseColumns()
                .WithColumn("ProjectId").AsGuid().NotNullable()
                .WithColumn("Name").AsString(256).NotNullable()
                .WithColumn("TTLInSeconds").AsInt32().NotNullable()
                .WithColumn("UsedSpaceInBytes").AsInt64().NotNullable()
                .WithColumn("FilesCount").AsInt64().NotNullable();
        }

        public override void Down()
        {
            Delete.Table("Storage");
        }
    }
}
