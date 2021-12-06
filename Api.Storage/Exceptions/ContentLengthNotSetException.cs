using System;

namespace Api.Storage.Exceptions
{
    [Serializable]
    public class ContentLengthNotSetException : Exception
    {
        public ContentLengthNotSetException()
        {
        }

        public ContentLengthNotSetException(string? message) : base(message)
        {
        }

        public ContentLengthNotSetException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
