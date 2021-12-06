using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Logging;

namespace Api.Storage.Http
{
    // TODO ВЫНЕСТИ В NUGET
    public class StorageFileResultExecutor : FileResultExecutorBase, IActionResultExecutor<StorageFileResult>
    {
        public StorageFileResultExecutor(ILogger<StorageFileResultExecutor> logger) : base(logger)
        {
        }

        public virtual async Task ExecuteAsync(ActionContext context, StorageFileResult result)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (result == null) throw new ArgumentNullException(nameof(result));

            await using (result.FileContent.FileStream)
            {
                var (_, rangeLength, serveBody) = SetHeadersAndLog(context, result, result.FileContent.FileSizeInBytes, true);

                if (!serveBody) // подробно см. в SetHeadersAndLog, это может быть 304 (NotModified) / 416 (Requested range not satisfiable)
                    return;
                
                try
                {
                    await StreamCopyOperation.CopyToAsync(result.FileContent.FileStream,
                        context.HttpContext.Response.Body,
                        rangeLength,
                        BufferSize,
                        context.HttpContext.RequestAborted);
                }
                catch (OperationCanceledException)
                {
                    // Don't throw this exception, it's most likely caused by the client disconnecting.
                    // However, if it was cancelled for any other reason we need to prevent empty responses.
                    context.HttpContext.Abort();
                }
            }
        }
    }
}