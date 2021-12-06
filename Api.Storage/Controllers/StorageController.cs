using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using HeyRed.Mime;
using Microsoft.Net.Http.Headers;

using Api.Storage.Http;
using Api.Storage.Services;
using Api.Storage.Models;

namespace Api.Storage.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [ApiVersion("1.0")]
    //TODO вернуть, когда добавится аутентификация в startup.cs
    //[Authorize]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public class StorageController : ControllerBase
    {
        private readonly IStorageService _storageService;
        private readonly IFileService _fileService;

        public StorageController(IStorageService storageService,
            IFileService fileService)
        {
            _storageService = storageService;
            _fileService = fileService;
        }

        [HttpPost]
        [ProducesResponseType(typeof(StorageInfo), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateNewStorage([FromBody] CreateStorageRequest createStorage)
        {
            var storageInfo = await _storageService.Create(createStorage);

            return Ok(storageInfo);
        }

        [HttpGet("{storageId}")]
        [ProducesResponseType(typeof(StorageInfo), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetStorageInfo(Guid storageId)
        {
            var storageInfo = await _storageService.Read(storageId);

            return Ok(storageInfo);
        }

        [HttpPut("{storageId}")]
        [ProducesResponseType(typeof(StorageInfo), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateStorageInfo(Guid storageId, [FromBody] StorageInfo storageInfo)
        {
            await _storageService.Update(storageInfo);

            return Ok();
        }

        [HttpDelete("{storageId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Delete(Guid storageId)
        {
            await _storageService.Delete(storageId);

            return NoContent();
        }

        [HttpGet("{storageId}/files/{fileId}")]
        public async Task<IActionResult> GetFile(Guid storageId, Guid fileId)
        {
            var rangeHeader = Request.GetTypedHeaders().Range;

            if (!IsRangeHeaderValid(rangeHeader))
            {
                var fileContent = await _fileService.GetFile(storageId, fileId);
                var contentType = MimeTypesMap.GetMimeType(fileContent.FileName);
                
                return File(fileContent.FileStream, contentType, fileContent.FileName, true);
            }

            var range = rangeHeader.Ranges.First();

            var startByte = range.From ?? 0;
            var endByte = range.To ?? 0;

            var filePartContent = await _fileService.GetFile(storageId, fileId, startByte, endByte);
            var filePartContentType = MimeTypesMap.GetMimeType(filePartContent.FileName);

            return new StorageFileResult(filePartContent, filePartContentType);
        }

        [HttpGet("{storageId}/files")]
        [ProducesResponseType(typeof(StorageContent), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllFiles(Guid storageId)
        {
            var storageContent = await _fileService.GetAll(storageId);

            return Ok(storageContent);
        }

        /// <summary>
        /// Выложить файл и получить его url (в теле запроса должен быть файл)
        /// </summary>
        /// <returns>url (https:// .... )</returns>
        [HttpPost("{storageId}/files")]
        [ProducesResponseType(typeof(StorageFileInfo), StatusCodes.Status200OK)]
        public async Task<IActionResult> UploadFile(Guid storageId, IFormFile file)
        // TODO IFormFile буферизуется либо в памяти, либо на диске; доделать стриминг
        // https://docs.microsoft.com/en-us/aspnet/core/mvc/models/file-uploads?view=aspnetcore-3.1#upload-large-files-with-streaming
        {
            if (string.IsNullOrWhiteSpace(file?.FileName))
                return BadRequest();

            var newFileParams = new NewFileParameters
            {
                FileName = WebUtility.HtmlEncode(file.FileName),
                FileSizeInBytes = file.Length,
                FileStream = file.OpenReadStream(),
            };

            using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(5));
            var fileInfo = await _fileService.Upload(storageId, newFileParams, cts.Token);

            return Ok(fileInfo);
        }

        [HttpDelete("{storageId}/files/{fileId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteFile(Guid storageId, Guid fileId)
        {
            await _fileService.Delete(storageId, fileId);

            return NoContent();
        }

        private bool IsRangeHeaderValid(RangeHeaderValue rangeHeader)
        {
            return rangeHeader != null && 
                   rangeHeader.Unit.HasValue &&
                   rangeHeader.Unit.Value == "bytes" &&
                   rangeHeader.Ranges.Count == 1; // пока несколько ренджей не поддерживаются
        }
    }
}
