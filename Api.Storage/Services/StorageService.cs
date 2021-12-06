using System;
using System.Threading.Tasks;
using AutoMapper;

using Api.DataAccess.Repositories;
using Api.Storage.Exceptions;
using Api.Storage.Models;

namespace Api.Storage.Services
{
    public class StorageService : IStorageService
    {
        private readonly IMapper _mapper;
        private readonly IStorageRepository _storageRepository;
        private readonly IFileService _fileService;
        private readonly IStorageProvider _storageProvider;

        public StorageService(IMapper mapper,
            IStorageRepository storageRepository,
            IFileService fileService,
            IStorageProvider storageProvider)
        {
            _mapper = mapper;
            _storageRepository = storageRepository;
            _fileService = fileService;
            _storageProvider = storageProvider;
        }

        public async Task<StorageInfo> Create(CreateStorageRequest createRequest)
        {
            var storage = _mapper.Map<DataAccess.Models.Storage>(createRequest);

            storage = await _storageRepository.Create(storage);

            if (storage == null)
                throw new CreateStorageException("Create storage error");

            await _storageProvider.Create(storage.Id);

            return _mapper.Map<StorageInfo>(storage);
        }

        public async Task<StorageInfo> Read(Guid storageId)
        {
            var storage = await _storageRepository.GetById(storageId);

            return _mapper.Map<StorageInfo>(storage);
        }

        public async Task Update(StorageInfo storageInfo)
        {
            var storage = _mapper.Map<DataAccess.Models.Storage>(storageInfo);

            var result = await _storageRepository.Update(storage);
            if (!result)
                throw new UpdateStorageException("Update storage error");
        }

        public async Task Delete(Guid storageId)
        {
            var storageContent = await _fileService.GetAll(storageId);
            foreach (var file in storageContent.Files)
            {
                await _fileService.Delete(storageId, file.Id);
            }

            await _storageProvider.Delete(storageId);

            var result = await _storageRepository.Delete(storageId);
            if (!result)
                throw new DeleteStorageException("Delete storage error");
        }
    }
}
