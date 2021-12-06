namespace Api.Storage.Configuration
{
    public class StorageSettings
    {
        public StorageType Type { get; set; }

        public string StorageAddress { get; set; }

        public string FileUrlTemplate { get; set; }

        public FileSystemSettings FileSystem { get; set; } = new FileSystemSettings();

        public S3Settings S3 { get; set; } = new S3Settings();
    }
}
