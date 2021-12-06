using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Options;
using Api.DataAccess.Models;
using Api.DataAccess.Repositories;
using Api.Storage.Configuration;
using Api.Storage.Models;

namespace Api.Storage.Services
{
    public class FileService : IFileService
    {
        private readonly StorageSettings _storageSettings;
        private readonly IMapper _mapper;
        private readonly IFileRepository _fileRepository;
        private readonly IStorageRepository _storageRepository;
        private readonly IFileProvider _fileProvider;
        private readonly IFileSystem _fileSystem;

        public FileService(IOptions<StorageSettings> storageSettings,
            IMapper mapper,
            IFileRepository fileRepository,
            IStorageRepository storageRepository,
            IFileProvider fileProvider,
            IFileSystem fileSystem)
        {
            _fileRepository = fileRepository;
            _storageRepository = storageRepository;
            _fileProvider = fileProvider;
            _fileSystem = fileSystem;
            _mapper = mapper;
            _storageSettings = storageSettings.Value;
        }

        public async Task<StorageFileContent> GetFile(Guid storageId, Guid fileId)
        {
            var file = await _fileRepository.GetById(fileId);
            if (file == null) throw new Exceptions.FileNotFoundException(fileId);

            var fileStream = await _fileProvider.Get(storageId, file.StoredFileName);

            return new StorageFileContent(fileStream, file.OriginalFileName, file.FileSizeInBytes);
        }

        public async Task<StorageFileContent> GetFile(Guid storageId, Guid fileId, long startByte, long endByte)
        {
            if (startByte < 0) throw new ArgumentOutOfRangeException(nameof(startByte));
            if (endByte < 0) throw new ArgumentOutOfRangeException(nameof(endByte));
            
            var file = await _fileRepository.GetById(fileId);
            if (file == null) throw new Exceptions.FileNotFoundException(fileId);

            if (endByte == 0)
                endByte = file.FileSizeInBytes;

            var fileStream = await _fileProvider.Get(storageId, file.StoredFileName, startByte, endByte);

            return new StorageFileContent(fileStream, file.OriginalFileName, file.FileSizeInBytes);
        }

        public async Task<StorageContent> GetAll(Guid storageId)
        {
            var files = await _fileRepository.GetAllByStorageId(storageId);

            var fileInfos = _mapper.Map<IEnumerable<StorageFileInfo>>(files).ToList();
            fileInfos.ForEach(fileInfo => fileInfo.FileLink = GetExternalLink(fileInfo));

            return new StorageContent
            {
                Files = fileInfos,
            };
        }

        public async Task<StorageFileInfo> Upload(Guid storageId, NewFileParameters newFileParams, CancellationToken token)
        {
            await _storageRepository.IncrementUsedSpace(storageId, newFileParams.FileSizeInBytes);

            try
            {
                var storageFile = new StorageFile
                {
                    StorageId = storageId,
                    OriginalFileName = newFileParams.FileName,
                    StoredFileName =  GetStoredFileName(newFileParams.FileName),
                    FileSizeInBytes = newFileParams.FileSizeInBytes,
                };

                storageFile = await _fileRepository.Create(storageFile);

                await _fileProvider.Upload(storageId, storageFile.StoredFileName, newFileParams.FileStream, token);

                var fileInfo = _mapper.Map<StorageFileInfo>(storageFile);
                fileInfo.FileLink = GetExternalLink(fileInfo);
                return fileInfo;
            }
            catch
            {
                await _storageRepository.DecrementUsedSpace(storageId, newFileParams.FileSizeInBytes);
                throw;
            }
        }
        
        public async Task Delete(Guid storageId, Guid fileId)
        {
            var file = await _fileRepository.GetById(fileId);
            if (file == null) throw new Exceptions.FileNotFoundException(fileId);

            await _fileProvider.Delete(storageId, file.StoredFileName);

            await _storageRepository.DecrementUsedSpace(storageId, file.FileSizeInBytes);

            await _fileRepository.Delete(fileId);
        }

        private string GetExternalLink(StorageFileInfo fileInfo)
        {
            return string.Format(_storageSettings.FileUrlTemplate,
                _storageSettings.StorageAddress,
                fileInfo.StorageId.ToString(),
                fileInfo.Id.ToString());
        }

        private string GetStoredFileName(string originalFileName)
        {
            return Guid.NewGuid() + _fileSystem.Path.GetExtension(originalFileName);
        }
    }
}
