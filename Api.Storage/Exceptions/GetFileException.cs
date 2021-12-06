using System;

namespace Api.Storage.Exceptions
{
    [Serializable]
    public class GetFileException : Exception
    {
        public GetFileException()
        {
        }

        public GetFileException(string message) : base(message)
        {
        }

        public GetFileException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
