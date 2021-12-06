using System;

namespace Api.Storage.Exceptions
{
    [Serializable]
    public class FileNotFoundException : Exception
    {
        public Guid FileId { get; }

        public FileNotFoundException()
        {
        }

        public FileNotFoundException(Guid fileId)
        {
            FileId = fileId;
        }

        public FileNotFoundException(string? message) : base(message)
        {
        }

        public FileNotFoundException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}