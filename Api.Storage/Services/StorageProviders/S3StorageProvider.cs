using System;
using System.Threading.Tasks;
using Amazon.S3;

namespace Api.Storage.Services.StorageProviders
{
    public class S3StorageProvider : IStorageProvider
    {
        private readonly IAmazonS3 _s3Client;

        public S3StorageProvider(IAmazonS3 s3Client)
        {
            _s3Client = s3Client;
        }

        public Task Create(Guid storageId)
        {
            return _s3Client.PutBucketAsync(storageId.ToString());
        }

        public Task Delete(Guid storageId)
        {
            return _s3Client.DeleteBucketAsync(storageId.ToString());
        }
    }
}
