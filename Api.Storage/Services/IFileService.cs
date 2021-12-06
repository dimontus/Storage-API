using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using Api.Storage.Models;

namespace Api.Storage.Services
{
    public interface IFileService
    {
        Task<StorageFileContent> GetFile(Guid storageId, Guid fileId);

        Task<StorageFileContent> GetFile(Guid storageId, Guid fileId, long startByte, long endByte);

        Task<StorageContent> GetAll(Guid storageId);

        Task<StorageFileInfo> Upload(Guid storageId, NewFileParameters newFileParams, CancellationToken token);
        
        Task Delete(Guid storageId, Guid fileId);
    }
}
