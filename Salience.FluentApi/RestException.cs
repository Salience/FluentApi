using System;
using System.Net;

namespace Salience.FluentApi
{
    public class RestException : Exception
    {
        public RestException()
        {
            
        }

        public RestException(string message)
            : base(message)
        {
            
        }

        public RestException(string message, string errorResponse, HttpStatusCode responseStatusCode)
            : base(message)
        {
            this.ErrorResponse = errorResponse;
            this.ResponseStatusCode = responseStatusCode;
        }

        public RestException(string message, Exception inner)
            : base(message, inner)
        {
        }

        public RestException(string message, Exception inner, HttpStatusCode responseStatusCode)
            : base(message, inner)
        {
            this.ResponseStatusCode = responseStatusCode;
        }

        public string ErrorResponse { get; private set; }
        public HttpStatusCode ResponseStatusCode { get; set; }
    }
}