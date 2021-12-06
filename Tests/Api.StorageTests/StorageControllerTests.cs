using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Moq;
using Api.DataAccess.Models;
using Api.DataAccess.Repositories;
using Api.Storage.Configuration;
using Api.Storage.Controllers;
using Api.Storage.Exceptions;
using Api.Storage.Http;
using Api.Storage.Models;
using Api.Storage.Services;
using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Threading.Tasks;
using Xunit;

namespace Api.StorageTests
{
    public class StorageControllerTests
    {
        public Mock<IMapper> MapperMock { get; } = new Mock<IMapper>();
        public Mock<IStorageRepository> StorageRepositoryMock { get; } = new Mock<IStorageRepository>();
        public Mock<IStorageProvider> StorageProviderMock { get; } = new Mock<IStorageProvider>();
        public Mock<IOptions<StorageSettings>> storageSettingsMock { get; } = new Mock<IOptions<StorageSettings>>();
        public Mock<IFileRepository> FileRepositoryMock { get; } = new Mock<IFileRepository>();
        public Mock<IFileProvider> FileProviderMock { get; } = new Mock<IFileProvider>();
        public Mock<IFileSystem> FileSystemMock { get; } = new Mock<IFileSystem>();
        public Mock<IFormFile> FormFileMock { get; } = new Mock<IFormFile>();
        public StorageController StorageController { get; }
        public StorageInfo StorageInfoEtalon { get; }
        public StorageFileInfo StorageFileInfoEtalon { get; }
        public Guid StorageId { get; } = Guid.NewGuid();
        public Guid FileId { get; } = Guid.NewGuid();

        public StorageControllerTests()
        {
            storageSettingsMock.Setup(k => k.Value).Returns(new StorageSettings { FileUrlTemplate = "template", StorageAddress = "address" });

            FileService FileService = new FileService(storageSettingsMock.Object,
                                                      MapperMock.Object,
                                                      FileRepositoryMock.Object,
                                                      StorageRepositoryMock.Object,
                                                      FileProviderMock.Object,
                                                      FileSystemMock.Object);

            StorageService StorageService = new StorageService(MapperMock.Object,
                                                               StorageRepositoryMock.Object,
                                                               FileService,
                                                               StorageProviderMock.Object);

            StorageController = new StorageController(StorageService, FileService);
            StorageInfoEtalon = new StorageInfo();
            StorageFileInfoEtalon = new StorageFileInfo();
        }
        private StorageFile BuildStorageFile()
        { return new StorageFile { OriginalFileName = "fileName", StoredFileName = "storedName", FileSizeInBytes = 100 }; }

        private void BuildOneHeader()
        {
            StorageController.ControllerContext.HttpContext = new DefaultHttpContext();
            StorageController.Request.Headers.Add("One", "bytes=1");
        }

        private void BuildRangeHeaders()
        {
            StorageController.ControllerContext.HttpContext = new DefaultHttpContext();
            StorageController.Request.Headers.Add("Range", "bytes=0-99");
        }

        [Fact]
        public async Task CreateNewStorage_WhenStorageCreateIsFailed_ThrowsCreateStorageException()
        {
            // Arrange
            StorageRepositoryMock.Setup(k => k.Create(It.IsAny<DataAccess.Models.Storage>())).ReturnsAsync((DataAccess.Models.Storage)null);

            // Act
            Task<IActionResult> Act() => StorageController.CreateNewStorage(new CreateStorageRequest());

            //Assert
            var ex = await Assert.ThrowsAsync<CreateStorageException>(Act);
            Assert.Equal("Create storage error", ex.Message);
        }

        [Fact]
        public async Task CreateNewStorage_WithCreateStorageRequest_ReturnsOkWithStorageInfo()
        {
            // Arrange
            StorageRepositoryMock.Setup(k => k.Create(It.IsAny<DataAccess.Models.Storage>())).ReturnsAsync(new DataAccess.Models.Storage());
            MapperMock.CreateMap<DataAccess.Models.Storage, CreateStorageRequest>(new CreateStorageRequest());
            MapperMock.CreateMap<DataAccess.Models.Storage, StorageInfo>(StorageInfoEtalon);

            // Act
            var result = await StorageController.CreateNewStorage(new CreateStorageRequest());

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(okResult.Value, StorageInfoEtalon);
        }

        [Fact]
        public async Task GetStorageInfo_WhenStorageIsNotFound_ReturnsOkWithNull()
        {
            // Arrange
            StorageRepositoryMock.Setup(k => k.GetById(It.IsAny<Guid>())).ReturnsAsync((DataAccess.Models.Storage)null);

            // Act
            var result = await StorageController.GetStorageInfo(Guid.NewGuid());

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Null(okResult.Value);
        }

        [Fact]
        public async Task GetStorageInfo_WithStorageId_ReturnsOkWithStorageInfo()
        {
            // Arrange
            StorageRepositoryMock.Setup(k => k.GetById(It.IsAny<Guid>())).ReturnsAsync((DataAccess.Models.Storage)null);
            MapperMock.CreateMap<DataAccess.Models.Storage, StorageInfo>(StorageInfoEtalon);

            // Act
            var result = await StorageController.GetStorageInfo(Guid.NewGuid());

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(okResult.Value, StorageInfoEtalon);
        }

        [Fact]
        public async Task UpdateStorageInfo_WhenUpdateIsFailed_ThrowsUpdateStorageException()
        {
            // Arrange
            MapperMock.CreateMap<DataAccess.Models.Storage, StorageInfo>(StorageInfoEtalon);
            StorageRepositoryMock.Setup(k => k.Update(It.IsAny<DataAccess.Models.Storage>())).ReturnsAsync(false);

            // Act
            Task<IActionResult> Act() => StorageController.UpdateStorageInfo(Guid.NewGuid(), StorageInfoEtalon);

            //Assert
            var ex = await Assert.ThrowsAsync<UpdateStorageException>(Act);
            Assert.Equal("Update storage error", ex.Message);
        }

        [Fact]
        public async Task UpdateStorageInfo_WithStorageIdAndStorageInfo_ReturnsOk()
        {
            // Arrange
            MapperMock.CreateMap<DataAccess.Models.Storage, StorageInfo>(StorageInfoEtalon);
            StorageRepositoryMock.Setup(k => k.Update(It.IsAny<DataAccess.Models.Storage>())).ReturnsAsync(true);

            // Act
            var result = await StorageController.UpdateStorageInfo(Guid.NewGuid(), StorageInfoEtalon);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task Delete_WhenDeleteIsFailed_ThrowsDeleteStorageException()
        {
            // Arrange
            StorageRepositoryMock.Setup(k => k.Delete(It.IsAny<Guid>())).ReturnsAsync(false);

            // Act 
            Task<IActionResult> Act() => StorageController.Delete(Guid.NewGuid());

            // Assert
            var ex = await Assert.ThrowsAsync<DeleteStorageException>(Act);
            Assert.Equal("Delete storage error", ex.Message);
        }

        [Fact]
        public async Task Delete_WhenDeleteIsFailed_ReturnsNoContent()
        {
            // Arrange
            StorageRepositoryMock.Setup(k => k.Delete(It.IsAny<Guid>())).ReturnsAsync(true);

            // Act
            var result = await StorageController.Delete(Guid.NewGuid());

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task GetFile_WhenHeadersIsNotRangeAndFileIdIsNotFound_ThrowFileNotFoundExceptionWithFileId()
        {
            // Arrange
            BuildOneHeader();
            FileRepositoryMock.Setup(k => k.GetById(It.IsAny<Guid>())).ReturnsAsync((StorageFile)null);

            // Act
            Task<IActionResult> Act() => StorageController.GetFile(StorageId, FileId);

            // Assert
            var ex = await Assert.ThrowsAsync<FileNotFoundException>(Act);
            Assert.Equal(FileId, ex.FileId);
            FileRepositoryMock.Verify(x => x.GetById(It.Is<Guid>(x => x == FileId)), Times.Once);
            FileProviderMock.Verify(x => x.Get(It.IsAny<Guid>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task GetFile_WithStorageIdAndFileIdWhenHeadersIsNotRange_ReturnsFileStreamResult()
        {
            // Arrange
            BuildOneHeader();
            var file = BuildStorageFile();
            var stream = new System.IO.MemoryStream();
            FileRepositoryMock.Setup(k => k.GetById(It.IsAny<Guid>())).ReturnsAsync(file);
            FileProviderMock.Setup(k => k.Get(It.IsAny<Guid>(), It.IsAny<string>())).ReturnsAsync(stream);

            // Act
            var result = await StorageController.GetFile(StorageId, FileId);

            // Assert
            Assert.IsType<FileStreamResult>(result);
            var fsResult = result as FileStreamResult;
            Assert.Equal(fsResult.FileStream, stream);
            Assert.Equal(fsResult.FileDownloadName, file.OriginalFileName);
            FileRepositoryMock.Verify(x => x.GetById(It.Is<Guid>(x => x == FileId)), Times.Once);
            FileProviderMock.Verify(x => x.Get(It.Is<Guid>(x => x == StorageId),
                                               It.Is<string>(x => x == file.StoredFileName)),
                                               Times.Once);
        }

        [Fact]
        public async Task GetFile_WhenHeadersIsRangeAndFileIdIsNotFound_ThrowFileNotFoundExceptionWithFileId()
        {
            // Arrange
            BuildRangeHeaders();
            FileRepositoryMock.Setup(k => k.GetById(It.IsAny<Guid>())).ReturnsAsync((StorageFile)null);

            // Act
            Task<IActionResult> Act() => StorageController.GetFile(StorageId, FileId);

            // Assert
            var ex = await Assert.ThrowsAsync<FileNotFoundException>(Act);
            Assert.Equal(FileId, ex.FileId);
            FileRepositoryMock.Verify(x => x.GetById(It.Is<Guid>(x => x == FileId)), Times.Once);
            FileProviderMock.Verify(x => x.Get(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<long>(), It.IsAny<long>()), Times.Never);

        }

        [Fact]
        public async Task GetFile_WithStorageIdAndFileIdWhenIsHeadersRange_ReturnsFileStreamResult()
        {
            // Arrange
            BuildRangeHeaders();
            var file = BuildStorageFile();
            var stream = new System.IO.MemoryStream();
            FileRepositoryMock.Setup(k => k.GetById(It.IsAny<Guid>())).ReturnsAsync(file);
            FileProviderMock.Setup(k => k.Get(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<long>(), It.IsAny<long>())).ReturnsAsync(stream);

            // Act
            var result = await StorageController.GetFile(StorageId, FileId);

            // Assert
            Assert.IsType<StorageFileResult>(result);
            var storageFileResult = result as StorageFileResult;
            Assert.Equal(storageFileResult.FileContent.FileName, file.OriginalFileName);
            Assert.Equal(storageFileResult.FileContent.FileStream, stream);
            Assert.Equal(storageFileResult.FileContent.FileSizeInBytes, file.FileSizeInBytes);
            FileRepositoryMock.Verify(x => x.GetById(It.Is<Guid>(x => x == FileId)), Times.Once);
            FileProviderMock.Verify(x => x.Get(It.Is<Guid>(x => x == StorageId),
                                               It.Is<string>(x => x == file.StoredFileName),
                                               It.Is<long>(x => x == 0),
                                               It.Is<long>(x => x == 99)),
                                               Times.Once);
        }

        [Fact]
        public async Task GetAllFiles_WithStorageId_ReturnsOkWithStorageContent()
        {
            // Arrange
            IEnumerable<StorageFileInfo> filesInfo = new List<StorageFileInfo> { new StorageFileInfo() };
            IEnumerable<StorageFile> files = new List<StorageFile> { new StorageFile() };
            FileRepositoryMock.Setup(k => k.GetAllByStorageId(It.IsAny<Guid>())).ReturnsAsync(files);
            MapperMock.CreateMap<IEnumerable<StorageFile>, IEnumerable<StorageFileInfo>>(filesInfo);

            // Act
            var result = await StorageController.GetAllFiles(Guid.NewGuid());

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.IsType<StorageContent>(okResult.Value);
        }

        [Fact]
        public async Task UploadFile_WhenFileNameIsNullOrWhiteSpace_ReturnsBadRequest()
        {
            // Arrange
            FormFileMock.Setup(k => k.FileName).Returns(String.Empty);

            // Act
            var result = await StorageController.UploadFile(Guid.NewGuid(), FormFileMock.Object);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task UploadFile_WithGuidAndFormFile_ReturnsOkWithFileInfo()
        {
            // Arrange
            FormFileMock.Setup(k => k.FileName).Returns("FileName");
            FileSystemMock.Setup(k => k.Path.GetExtension(It.IsAny<string>())).Returns(String.Empty);
            FileRepositoryMock.Setup(k => k.Create(It.IsAny<StorageFile>())).ReturnsAsync(new StorageFile());
            MapperMock.CreateMap<StorageFile, StorageFileInfo>(StorageFileInfoEtalon);

            // Act
            var result = await StorageController.UploadFile(Guid.NewGuid(), FormFileMock.Object);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(okResult.Value, StorageFileInfoEtalon);
        }

        [Fact]
        public async Task DeleteFile_WhenFileIsNotFound_ThrowFileNotFoundExceptionWithFileId()
        {
            // Arrange
            FileRepositoryMock.Setup(k => k.GetById(It.IsAny<Guid>())).ReturnsAsync((StorageFile)null);

            // Act
            Task<IActionResult> Act() => StorageController.DeleteFile(StorageId, FileId);

            // Assert
            var ex = await Assert.ThrowsAsync<FileNotFoundException>(Act);
            FileRepositoryMock.Verify(x => x.GetById(It.Is<Guid>(x => x == FileId)), Times.Once);
            FileRepositoryMock.Verify(x => x.Delete(It.IsAny<Guid>()), Times.Never);

        }

        [Fact]
        public async Task DeleteFile_WithStorageIdAndFileId_ReturnsNoContent()
        {
            // Arrange
            FileRepositoryMock.Setup(k => k.GetById(It.IsAny<Guid>())).ReturnsAsync(new StorageFile());

            // Act
            var result = await StorageController.DeleteFile(StorageId, FileId);

            // Assert
            Assert.IsType<NoContentResult>(result);
            FileRepositoryMock.Verify(x => x.GetById(It.Is<Guid>(x => x == FileId)), Times.Once);
            FileRepositoryMock.Verify(x => x.Delete(It.Is<Guid>(x => x == FileId)), Times.Once);
        }

    }
}
