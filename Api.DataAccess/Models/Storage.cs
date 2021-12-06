using System;
using DataAccess;

namespace Api.DataAccess.Models
{
    public class Storage : EntityBase
    {
        public Guid ProjectId { get; set; }
        public string Name { get; set; }
        public int TTLInSeconds { get; set; }
        public long UsedSpaceInBytes { get; set; }
        public long FilesCount { get; set; }
    }
}
