using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Api.Storage.Services
{
    public interface IFileProvider
    {
        Task<Stream> Get(Guid storageId, string fileName);

        Task<Stream> Get(Guid storageId, string fileName, long startByte, long endByte);

        Task Upload(Guid storageId, string fileName, Stream stream, CancellationToken token);

        Task Delete(Guid storageId, string fileName);
    }
}
