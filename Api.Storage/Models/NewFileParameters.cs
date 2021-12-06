using System.IO;

namespace Api.Storage.Models
{
    public class NewFileParameters
    {
        public string FileName { get; set; }

        public long FileSizeInBytes { get; set; }

        public Stream FileStream { get; set; }
    }
}
