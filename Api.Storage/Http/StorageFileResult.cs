using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

using Api.Storage.Models;

namespace Api.Storage.Http
{
    // TODO ВЫНЕСТИ В NUGET
    public class StorageFileResult : FileResult
    {
        public StorageFileResult(StorageFileContent fileContent, string contentType) : base(contentType)
        {
            FileContent = fileContent ?? throw new ArgumentNullException(nameof(fileContent));
            FileDownloadName = fileContent.FileName;
        }

        public StorageFileContent FileContent { get; }

        /// <inheritdoc />
        public override Task ExecuteResultAsync(ActionContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var executor = context.HttpContext.RequestServices.GetRequiredService<IActionResultExecutor<StorageFileResult>>();
            return executor.ExecuteAsync(context, this);
        }
    }
}