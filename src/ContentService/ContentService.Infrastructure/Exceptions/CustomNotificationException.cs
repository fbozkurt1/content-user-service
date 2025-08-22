using Microsoft.AspNetCore.Http;
using System;

namespace ContentService.Infrastructure.Exceptions
{
    public class CustomNotificationException : Exception
    {
        public int StatusCode { get; set; } 
        public CustomNotificationException(string message, int statusCode = StatusCodes.Status500InternalServerError)
            :base(message)
        {
            StatusCode = statusCode;
        }
    }
}
