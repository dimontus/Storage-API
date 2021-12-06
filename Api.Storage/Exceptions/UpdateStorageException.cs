using System;

namespace Api.Storage.Exceptions
{
    [Serializable]
    public class UpdateStorageException : Exception
    {
        public UpdateStorageException()
        {
        }

        public UpdateStorageException(string? message) : base(message)
        {
        }

        public UpdateStorageException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
