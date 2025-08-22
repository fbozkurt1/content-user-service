using Microsoft.AspNetCore.Http;

namespace UserService.Infrastructure.Exceptions
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
