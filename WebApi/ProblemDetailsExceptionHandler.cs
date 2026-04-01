using System;
using System.Threading;
using System.Threading.Tasks;
using AppCore.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Logging;

namespace WebApi;

public class ProblemDetailsExceptionHandler(
    ProblemDetailsFactory factory, ILogger<ProblemDetailsExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is ContactNotFoundException)
        {
            logger.Log(LogLevel.Information, $"Exception '{exception.Message}' handled!");
            var problem = factory.CreateProblemDetails(
                context,
                StatusCodes.Status400BadRequest,
                "Contact service error!",
                "Service error",
                detail: exception.Message
            );
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsJsonAsync(problem);
            return true;
        }
        return false;
    }
}