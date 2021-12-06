using System;
using System.Threading.Tasks;
using Api.Storage.Models;

namespace Api.Storage.Services
{
    public interface IStorageService
    {
        Task<StorageInfo> Create(CreateStorageRequest createRequest);
        Task<StorageInfo> Read(Guid storageId);
        Task Update(StorageInfo storageInfo);
        Task Delete(Guid storageId);
    }
}
