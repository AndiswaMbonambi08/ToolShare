using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ToolShare.Domain.Exceptions;

namespace ToolShare.ErrorHandling;

public class ToolShareExceptionHandler : IExceptionHandler
{
    private readonly ILogger<ToolShareExceptionHandler> _logger;

    public ToolShareExceptionHandler(ILogger<ToolShareExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var (statusCode, title) = exception switch
        {
            ToolShareException tse => (tse.StatusCode, tse.Title),
            ArgumentException => (StatusCodes.Status400BadRequest, "Bad Request"),
            _ => (StatusCodes.Status500InternalServerError, "Internal Server Error")
        };

        var detail = statusCode == StatusCodes.Status500InternalServerError
            ? "An unexpected error occurred."
            : exception.Message;

        if (statusCode >= StatusCodes.Status500InternalServerError)
        {
            _logger.LogError(exception, "Unhandled error. Method: {Method}, Path: {Path}, StatusCode: {StatusCode}",
                httpContext.Request.Method, httpContext.Request.Path, statusCode);
        }
        else
        {
            _logger.LogInformation("Request rejected. Method: {Method}, Path: {Path}, StatusCode: {StatusCode}, Reason: {Reason}",
                httpContext.Request.Method, httpContext.Request.Path, statusCode, exception.Message);
        }

        var problemDetails = new ProblemDetails { Status = statusCode, Title = title, Detail = detail };

        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, options: (System.Text.Json.JsonSerializerOptions?)null, contentType: "application/problem+json", cancellationToken);

        return true;
    }
}