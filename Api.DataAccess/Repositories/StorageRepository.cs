using System;
using System.Threading.Tasks;
using AspNetCore.Http;
using DataAccess;
using DataAccess.Configuration;
using DataAccess.Configuration.Schemes;
using Dapper;
using Microsoft.Extensions.Options;
using Api.DataAccess.Exceptions;
using Api.DataAccess.Models;

namespace Api.DataAccess.Repositories
{
    public class StorageRepository : BaseRepository<Storage>, IStorageRepository
    {
        public StorageRepository(IOptionsSnapshot<TenantDbOptions> dbOptions, ICurrentUserAccessor currentUserAccessor) : 
            base(dbOptions, currentUserAccessor, TenantSchemes.Storage)
        {
        }

        public async Task IncrementUsedSpace(Guid storageId, long spaceToReserveInBytes)
        {
            await using var db = await GetSqlConnection();

            var result = await db.ExecuteAsync($"UPDATE {TableName} " +
                                             $"SET [UsedSpaceInBytes] = [UsedSpaceInBytes] + @spaceToReserveInBytes " +
                                             $"WHERE [Id] = @storageId AND [IsDeleted] = 0 ", 
                new { storageId, spaceToReserveInBytes });

            if (result != 1)
                throw new UsedSpaceUpdateException();
        }

        public async Task DecrementUsedSpace(Guid storageId, long spaceToFreeInBytes)
        {
            await using var db = await GetSqlConnection();

            var result = await db.ExecuteAsync($"UPDATE {TableName} " +
                                               $"SET [UsedSpaceInBytes] = [UsedSpaceInBytes] - @spaceToFreeInBytes " +
                                               $"WHERE [Id] = @storageId AND [IsDeleted] = 0 ",
                new { storageId, spaceToFreeInBytes });

            if (result != 1)
                throw new UsedSpaceUpdateException();
        }
    }
}
