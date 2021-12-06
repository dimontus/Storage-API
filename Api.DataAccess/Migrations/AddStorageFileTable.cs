using FluentMigrator;

namespace Api.DataAccess.Migrations
{
    [Migration(20200304182100)]
    public class AddStorageFileTable : Migration
    {
        public override void Up()
        {
            Create.Table("StorageFile")
                .WithEntityBaseColumns()
                .WithColumn("StorageId").AsGuid().NotNullable()
                .WithColumn("OriginalFileName").AsString(256).NotNullable()
                .WithColumn("StoredFileName").AsString(256).NotNullable()
                .WithColumn("FileSizeInBytes").AsInt64().NotNullable();
        }

        public override void Down()
        {
            Delete.Table("StorageFile");
        }
    }
}
