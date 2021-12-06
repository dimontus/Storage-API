using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataAccess;
using Api.DataAccess.Models;

namespace Api.DataAccess.Repositories
{
    public interface IFileRepository : IBaseRepository<StorageFile>
    {
        Task<IEnumerable<StorageFile>> GetAllByStorageId(Guid storageId);
    }
}
