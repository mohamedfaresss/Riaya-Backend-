using System;
using System.Collections.Generic;
using System.Text;

namespace Riaya.Domain.Exceptions
{
    public class ForbiddenException : Exception
    {
        public ForbiddenException(string message) : base(message) { }
    }
    }