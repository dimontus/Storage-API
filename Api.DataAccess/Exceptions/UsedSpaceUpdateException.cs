using System;

namespace Api.DataAccess.Exceptions
{
    [Serializable]
    public class UsedSpaceUpdateException : Exception
    {
        public UsedSpaceUpdateException()
        {
        }

        public UsedSpaceUpdateException(string? message) : base(message)
        {
        }

        public UsedSpaceUpdateException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
