using System;
using System.IO;

namespace Api.Storage.Models
{
    public class StorageFileContent
    {
        public StorageFileContent(Stream fileStream, string fileName, long fileSizeInBytes)
        {
            if (fileSizeInBytes <= 0) throw new ArgumentOutOfRangeException(nameof(fileSizeInBytes));

            FileStream = fileStream ?? throw new ArgumentNullException(nameof(fileStream));
            FileName = fileName ?? throw new ArgumentNullException(nameof(fileName));
            FileSizeInBytes = fileSizeInBytes;
        }

        public Stream FileStream { get; }

        public string FileName { get; }

        public long FileSizeInBytes { get; }
    }
}