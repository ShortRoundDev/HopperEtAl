using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibJohn.Exceptions
{
    public class BadFileFormatException : Exception
    {
        public BadFileFormatException() : this("File contained bad header. Expected 'FUK'")
        {
        }

        public BadFileFormatException(string? message) : base(message)
        {
        }

        public BadFileFormatException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
