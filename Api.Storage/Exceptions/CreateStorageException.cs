using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Storage.Exceptions
{
    public class CreateStorageException : Exception
    {
        public CreateStorageException()
        {
        }

        public CreateStorageException(string? message) : base(message)
        {
        }

        public CreateStorageException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
