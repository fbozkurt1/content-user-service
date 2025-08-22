using Microsoft.AspNetCore.Diagnostics;
using UserService.Infrastructure.Exceptions;

namespace UserService.API.Infrastructure
{
    public class ExceptionHandler : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext,
                                                    Exception exception,
                                                    CancellationToken cancellationToken)
        {

            if (exception is CustomNotificationException)
            {
                var ex = exception as CustomNotificationException;
                httpContext.Response.StatusCode = ex!.StatusCode;

                var baseResponse = new BaseResponse(ex.Message);
                await httpContext.Response.WriteAsJsonAsync(baseResponse, cancellationToken);
            }
            else
            {
                httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

                var baseResponse = new BaseResponse("An error has occured. Please try again later.");
                await httpContext.Response.WriteAsJsonAsync(baseResponse, cancellationToken);
            }
            return true;
        }
    }
}
