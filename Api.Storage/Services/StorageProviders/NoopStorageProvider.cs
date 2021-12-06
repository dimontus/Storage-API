using System;
using System.Threading.Tasks;

namespace Api.Storage.Services.StorageProviders
{
    public class NoopStorageProvider : IStorageProvider
    {
        public Task Create(Guid storageId)
        {
            return Task.CompletedTask;
        }

        public Task Delete(Guid storageId)
        {
            return Task.CompletedTask;
        }
    }
}
