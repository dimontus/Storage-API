using System;
using System.IO.Abstractions;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Api.Storage.Configuration;

namespace Api.Storage.Services.StorageProviders
{
    public class FileSystemStorageProvider : IStorageProvider
    {
        private readonly StorageSettings _storageSettings;
        private readonly IFileSystem _fileSystem;

        public FileSystemStorageProvider(IOptions<StorageSettings> storageSettings,
            IFileSystem fileSystem)
        {
            _storageSettings = storageSettings.Value;
            _fileSystem = fileSystem;

            if (!_fileSystem.Directory.Exists(_storageSettings.FileSystem.BasePath))
            {
                _fileSystem.Directory.CreateDirectory(_storageSettings.FileSystem.BasePath);
            }
        }

        public Task Create(Guid storageId)
        {
            var storageDirectoryPath = GetStorageDirectoryPath(storageId);

            if (!_fileSystem.Directory.Exists(storageDirectoryPath))
            {
                _fileSystem.Directory.CreateDirectory(storageDirectoryPath);
            }

            return Task.CompletedTask;
        }

        public Task Delete(Guid storageId)
        {
            var storageDirectoryPath = GetStorageDirectoryPath(storageId);

            if (_fileSystem.Directory.Exists(storageDirectoryPath))
            {
                _fileSystem.Directory.Delete(storageDirectoryPath, true);
            }

            return Task.CompletedTask;
        }

        private string GetStorageDirectoryPath(Guid storageId)
        {
            return _fileSystem.Path.Combine(_storageSettings.FileSystem.BasePath, storageId.ToString());
        }
    }
}
