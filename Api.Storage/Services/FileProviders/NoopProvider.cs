using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.WebUtilities;

namespace Api.Storage.Services.FileProviders
{
    public class NoopProvider : IFileProvider
    {
        public Task<Stream> Get(Guid storageId, string fileName)
        {
            return Task.FromResult(Stream.Null);
        }

        public Task<Stream> Get(Guid storageId, string fileName, long startByte, long endByte)
        {
            return Task.FromResult(Stream.Null);
        }

        public async Task Upload(Guid storageId, string fileName, Stream stream, CancellationToken token)
        {
            await stream.DrainAsync(token);
        }

        public Task Delete(Guid storageId, string fileName)
        {
            return Task.CompletedTask;
        }
    }
}
