using System;
using System.Threading.Tasks;

namespace Api.Storage.Services
{
    public interface IStorageProvider
    {
        Task Create(Guid storageId);
        Task Delete(Guid storageId);
    }
}
