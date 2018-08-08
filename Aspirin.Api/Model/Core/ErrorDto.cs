using System.Collections.Generic;

namespace Aspirin.Api.Model.Core
{
    public class ErrorDto
    {
        public ErrorDto(int statusCode, List<string> messages)
        {
            StatusCode = statusCode;
            Messages = messages;
        }

        public int StatusCode { get; }
        public List<string> Messages { get; }
    }
}
