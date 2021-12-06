using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AspNetCore.Http;
using DataAccess;
using DataAccess.Configuration;
using DataAccess.Configuration.Schemes;
using Microsoft.Extensions.Options;
using Dapper;
using Api.DataAccess.Models;

namespace Api.DataAccess.Repositories
{
    public class FileRepository : BaseRepository<StorageFile>, IFileRepository
    {
        public FileRepository(IOptionsSnapshot<TenantDbOptions> dbOptions, ICurrentUserAccessor currentUserAccessor) : 
            base(dbOptions, currentUserAccessor, TenantSchemes.Storage)
        {
        }

        public async Task<IEnumerable<StorageFile>> GetAllByStorageId(Guid storageId)
        {
            await using var db = await GetSqlConnection();
            return await db.QueryAsync<StorageFile>(
                $"SELECT * FROM {TableName} WHERE [StorageId] = @storageId AND IsDeleted = 0", new { storageId });
        }
    }
}
