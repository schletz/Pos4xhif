using System;
using System.Collections.Generic;
using System.Text;

namespace Plf4ahif20191118.Testapp
{
    class HttpException : Exception
    {
        public int Status { get; set; }
        public HttpException() : base() { }
        public HttpException(string message) : base(message) { }
        public HttpException(string message, Exception innerException) : base(message, innerException) { }
    }
}
