using System;

namespace Api.Storage.Models
{
    public class StorageInfo
    {
        public Guid Id { get; set; }
        public Guid ProjectId { get; set; }
        public string Name { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int TTLInSeconds { get; set; }
        public long UsedSpaceInBytes { get; set; }
        public long FreeSpaceInBytes { get; set; }
        public long FilesCount { get; set; }
    }
}
