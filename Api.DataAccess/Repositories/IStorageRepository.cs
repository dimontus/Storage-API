using System;
using System.Threading.Tasks;
using DataAccess;
using Api.DataAccess.Models;

namespace Api.DataAccess.Repositories
{
    public interface IStorageRepository : IBaseRepository<Storage>
    {
        Task IncrementUsedSpace(Guid storageId, long spaceToReserveInBytes);

        Task DecrementUsedSpace(Guid storageId, long spaceToFreeInBytes);
    }
}
