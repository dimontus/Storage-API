using FluentMigrator.Builders.Create.Table;

namespace Api.DataAccess.Migrations
{
    public static class EntityBaseExtensions
    {
        // TODO ВЫНЕСТИ В NUGET
        public static ICreateTableWithColumnSyntax WithEntityBaseColumns(this ICreateTableWithColumnOrSchemaOrDescriptionSyntax table)
        {
            return table
                .WithColumn("Id").AsGuid().NotNullable().PrimaryKey()
                .WithColumn("CreatedBy").AsGuid().NotNullable()
                .WithColumn("CreatedDate").AsDateTime().NotNullable()
                .WithColumn("LastSavedBy").AsGuid().NotNullable()
                .WithColumn("LastSavedDate").AsDateTime().NotNullable()
                .WithColumn("IsDeleted").AsBoolean().NotNullable();
        }
    }
}
