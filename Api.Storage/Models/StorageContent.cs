using System.Collections.Generic;

namespace Api.Storage.Models
{
    public class StorageContent
    {
        public IList<StorageFileInfo> Files { get; set; }
    }
}
