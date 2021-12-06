using System;
using System.IO;
using System.IO.Abstractions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Api.Storage.Configuration;

namespace Api.Storage.Services.FileProviders
{
    public class FileSystemProvider : IFileProvider
    {
        private readonly StorageSettings _storageSettings;
        private readonly IFileSystem _fileSystem;

        public FileSystemProvider(IOptions<StorageSettings> storageSettings,
            IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
            _storageSettings = storageSettings.Value;

            if (!_fileSystem.Directory.Exists(_storageSettings.FileSystem.BasePath))
            {
                _fileSystem.Directory.CreateDirectory(_storageSettings.FileSystem.BasePath);
            }
        }

        public Task<Stream> Get(Guid storageId, string fileName)
        {
            var filePath = GetFilePath(storageId, fileName);
            return Task.FromResult(_fileSystem.File.OpenRead(filePath));
        }

        public Task<Stream> Get(Guid storageId, string fileName, long startByte, long endByte)
        {
            if (startByte < 0) throw new ArgumentOutOfRangeException(nameof(startByte));
            if (endByte <= 0 || endByte < startByte) throw new ArgumentOutOfRangeException(nameof(endByte));

            var filePath = GetFilePath(storageId, fileName);
            var stream = _fileSystem.File.OpenRead(filePath);
            
            stream.Seek(startByte, SeekOrigin.Begin);

            return Task.FromResult(stream);
        }

        public async Task Upload(Guid storageId, string fileName, Stream stream, CancellationToken token)
        {
            var filePath = GetFilePath(storageId, fileName);
            await using var writeStream = _fileSystem.File.Open(filePath, FileMode.Create);

            await stream.CopyToAsync(writeStream, token);
            await writeStream.FlushAsync(token);
        }

        public Task Delete(Guid storageId, string fileName)
        {
            var filePath = GetFilePath(storageId, fileName);
            _fileSystem.File.Delete(filePath);
            return Task.CompletedTask;
        }

        private string GetFilePath(Guid storageId, string fileName)
        {
            return _fileSystem.Path.Combine(_storageSettings.FileSystem.BasePath, storageId.ToString(), fileName);
        }
    }
}
