using System;

namespace Api.Storage.Models
{
    public class StorageFileInfo
    {
        public Guid Id { get; set; }
        public Guid StorageId { get; set; }
        public string FileName { get; set; }
        public string FileLink { get; set; }
        public long FileSizeInBytes { get; set; }
    }
}
