using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibJohn.Exceptions
{
    public class EmptyFileException : Exception
    {
        public EmptyFileException() : this("TileMap file contained no data")
        {
        }

        public EmptyFileException(string? message) : base(message)
        {
        }

        public EmptyFileException(string? message, Exception? innerException) : base(message, innerException)
        {

        }
    }
}
