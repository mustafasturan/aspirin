using System;

namespace Aspirin.Api.Model.Core
{
    public class AspirinException : Exception
    {
        public AspirinException(string message, int statusCode) : base(message)
        {
            StatusCode = statusCode;
        }
        public int StatusCode { get; set; }
    }
}
