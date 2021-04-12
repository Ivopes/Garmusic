using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Garmusic.Utilities.Exceptions
{
    public class NoAccountFoundException : Exception
    {
        public NoAccountFoundException()
        {
        }

        public NoAccountFoundException(string message) : base(message)
        {
        }

        public NoAccountFoundException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
