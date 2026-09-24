using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SmartWarehouse.Domain.Exceptions;
using System.Text.Json;

namespace SmartWarehouse.API.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred.");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/problem+json";

        var problemDetails = new ProblemDetails
        {
            Instance = context.Request.Path
        };

        switch (exception)
        {
            case NotFoundException notFoundException:
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                problemDetails.Title = "Not Found";
                problemDetails.Status = StatusCodes.Status404NotFound;
                problemDetails.Detail = notFoundException.Message;
                break;
            case BadRequestException badRequestException:
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                problemDetails.Title = "Bad Request";
                problemDetails.Status = StatusCodes.Status400BadRequest;
                problemDetails.Detail = badRequestException.Message;
                break;
            case BusinessRuleException businessRuleException:
                context.Response.StatusCode = StatusCodes.Status422UnprocessableEntity;
                problemDetails.Title = "Business Rule Violation";
                problemDetails.Status = StatusCodes.Status422UnprocessableEntity;
                problemDetails.Detail = businessRuleException.Message;
                break;
            case ConcurrencyException concurrencyException:
                context.Response.StatusCode = StatusCodes.Status409Conflict;
                problemDetails.Title = "Concurrency Conflict";
                problemDetails.Status = StatusCodes.Status409Conflict;
                problemDetails.Detail = concurrencyException.Message;
                break;
            case ForbiddenException forbiddenException:
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                problemDetails.Title = "Forbidden";
                problemDetails.Status = StatusCodes.Status403Forbidden;
                problemDetails.Detail = forbiddenException.Message;
                break;
            case UnauthorizedAccessException unauthorizedAccessException:
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                problemDetails.Title = "Unauthorized";
                problemDetails.Status = StatusCodes.Status401Unauthorized;
                problemDetails.Detail = unauthorizedAccessException.Message;
                break;
            // FluentValidation ValidationException could be handled here too (we'll map it to 400 Bad Request)
            case FluentValidation.ValidationException validationException:
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                problemDetails.Title = "Validation Failed";
                problemDetails.Status = StatusCodes.Status400BadRequest;
                problemDetails.Detail = "One or more validation errors occurred.";
                problemDetails.Extensions["errors"] = validationException.Errors
                    .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
                    .ToDictionary(g => g.Key, g => g.ToArray());
                break;
            default:
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                problemDetails.Title = "Internal Server Error";
                problemDetails.Status = StatusCodes.Status500InternalServerError;
                problemDetails.Detail = "An unexpected error occurred.";
                break;
        }

        var json = JsonSerializer.Serialize(problemDetails);
        await context.Response.WriteAsync(json);
    }
}
