using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using HeyRed.Mime;

using Api.Storage.Exceptions;

namespace Api.Storage.Services.FileProviders
{
    public class S3Provider : IFileProvider
    {
        private readonly IAmazonS3 _s3Client;

        public S3Provider(IAmazonS3 s3Client)
        {
            _s3Client = s3Client;
        }

        public async Task<Stream> Get(Guid storageId, string fileName)
        {
            var response = await _s3Client.GetObjectAsync(storageId.ToString(), fileName);

            return response.ResponseStream;
        }

        public async Task<Stream> Get(Guid storageId, string fileName, long startByte, long endByte)
        {
            if (startByte < 0) throw new ArgumentOutOfRangeException(nameof(startByte));
            if (endByte <= 0 || endByte < startByte) throw new ArgumentOutOfRangeException(nameof(endByte));

            var request = new GetObjectRequest
            {
                BucketName = storageId.ToString(),
                Key = fileName,
                ByteRange = new ByteRange(startByte, endByte),
            };

            var response = await _s3Client.GetObjectAsync(request);

            return response.ResponseStream;
        }

        public Task Upload(Guid storageId, string fileName, Stream stream, CancellationToken token)
        {
            var request = new PutObjectRequest
            {
                BucketName = storageId.ToString(),
                Key = fileName,
                InputStream = stream,
                ContentType = MimeTypesMap.GetMimeType(fileName),
            };

            return _s3Client.PutObjectAsync(request, token);
        }

        public Task Delete(Guid storageId, string fileName)
        {
            return _s3Client.DeleteObjectAsync(storageId.ToString(), fileName);
        }
    }
}
