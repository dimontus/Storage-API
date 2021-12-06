using System;

namespace Api.Storage.Exceptions
{
    [Serializable]
    public class DeleteStorageException : Exception
    {
        public DeleteStorageException()
        {
        }

        public DeleteStorageException(string message) : base(message)
        {
        }

        public DeleteStorageException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
