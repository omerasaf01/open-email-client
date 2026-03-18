using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Common;
using System.Net.Sockets;
using System.Security;
using System.Security.Authentication;
using System.Text.Json;
using System.Transactions;
using Microsoft.EntityFrameworkCore;

namespace OpenEmail.Api.Middlewares;

/// <summary>
/// Error response model for exception handling
/// </summary>
public sealed class ErrorResponse
{
    public string Status { get; set; } = null!;
    public int Code { get; set; }
    public string Reason { get; set; } = null!;
    public string? Note { get; set; }
    public string? ExceptionType { get; set; }
    public string? Route { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Exception handler middleware options
/// </summary>
public sealed class ExceptionHandlerOptions
{
    /// <summary>
    /// Set to true if you'd like to log the error in a structured manner
    /// </summary>
    public bool LogStructuredException { get; set; } = true;

    /// <summary>
    /// Set to true if you don't want to expose the actual exception reason in the json response sent to the client
    /// </summary>
    public bool UseGenericReason { get; set; } = false;

    /// <summary>
    /// Include exception type in response (useful for debugging, disable in production)
    /// </summary>
    public bool IncludeExceptionType { get; set; } = true;

    /// <summary>
    /// Include route information in response
    /// </summary>
    public bool IncludeRoute { get; set; } = true;
}

/// <summary>
/// Custom exception handler middleware that combines FastEndpoints style with detailed exception mapping
/// </summary>
public class ExceptionHandlerMiddleware(
    ILogger<ExceptionHandlerMiddleware> logger,
    ExceptionHandlerOptions? options = null) : IMiddleware
{
    private readonly ExceptionHandlerOptions _options = options ?? new ExceptionHandlerOptions();

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var exceptionType = exception.GetType().Name;
        var route = context.GetEndpoint()?.DisplayName?.Split(" => ").FirstOrDefault()
                    ?? context.Request.Path.Value;
        var reason = exception.Message;

        // Log the exception (structured or unstructured)
        if (_options.LogStructuredException)
        {
            logger.LogError(
                exception,
                "Exception: {ExceptionType} | Route: {Route} | Reason: {Reason}",
                exceptionType,
                route,
                reason);
        }
        else
        {
            logger.LogError(
                "=================================\n" +
                "Exception: {ExceptionType}\n" +
                "Route: {Route}\n" +
                "Reason: {Reason}\n" +
                "StackTrace: {StackTrace}\n" +
                "=================================",
                exceptionType,
                route,
                reason,
                exception.StackTrace);
        }

        var (statusCode, statusMessage, defaultReason) = GetExceptionDetails(exception);

        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/problem+json";

        var errorResponse = new ErrorResponse
        {
            Status = statusMessage,
            Code = statusCode,
            Reason = _options.UseGenericReason ? defaultReason : reason,
            Note = "See application log for stack trace.",
            ExceptionType = _options.IncludeExceptionType ? exceptionType : null,
            Route = _options.IncludeRoute ? route : null
        };

        await context.Response.WriteAsJsonAsync(errorResponse, JsonOptions, context.RequestAborted);
    }

    private static (int StatusCode, string StatusMessage, string DefaultReason) GetExceptionDetails(Exception exception)
    {
        return exception switch
        {
            // 400 Bad Request
            ArgumentNullException => (StatusCodes.Status400BadRequest, "Bad Request", "Null argument is not allowed."),
            ArgumentException => (StatusCodes.Status400BadRequest, "Bad Request", "Invalid argument passed."),
            FormatException => (StatusCodes.Status400BadRequest, "Bad Request", "Data format is incorrect."),
            InvalidCastException => (StatusCodes.Status400BadRequest, "Bad Request", "Invalid type conversion."),
            OverflowException => (StatusCodes.Status400BadRequest, "Bad Request", "Numeric value overflow."),
            IndexOutOfRangeException => (StatusCodes.Status400BadRequest, "Bad Request", "Index is out of range."),
            ValidationException => (StatusCodes.Status400BadRequest, "Bad Request", "Validation failed. Please check the input data for any errors."),
            InvalidDataException => (StatusCodes.Status400BadRequest, "Bad Request", "Invalid data."),

            // 401 Unauthorized
            UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, "Unauthorized", "Unauthorized access."),
            AuthenticationException => (StatusCodes.Status401Unauthorized, "Unauthorized", "Authentication failed."),

            // 403 Forbidden
            AccessViolationException => (StatusCodes.Status403Forbidden, "Forbidden", "Access violation detected."),
            SecurityException => (StatusCodes.Status403Forbidden, "Forbidden", "Security violation detected."),

            // 404 Not Found
            KeyNotFoundException => (StatusCodes.Status404NotFound, "Not Found", "Key not found."),
            FileNotFoundException => (StatusCodes.Status404NotFound, "Not Found", "File not found."),
            DirectoryNotFoundException => (StatusCodes.Status404NotFound, "Not Found", "Directory not found."),

            // 408 Request Timeout
            HttpRequestException => (StatusCodes.Status408RequestTimeout, "Request Timeout", "Request timed out."),

            // 502 Bad Gateway (External service connection errors)
            SocketException => (StatusCodes.Status502BadGateway, "Bad Gateway", "Unable to connect to external service."),

            // 409 Conflict
            InvalidOperationException => (StatusCodes.Status409Conflict, "Conflict", "Invalid operation."),
            DbUpdateConcurrencyException => (StatusCodes.Status409Conflict, "Conflict", "Concurrency conflict (another user modified the same data)."),

            // 422 Unprocessable Entity
            DivideByZeroException => (StatusCodes.Status422UnprocessableEntity, "Unprocessable Entity", "Attempted to divide by zero."),
            DataMisalignedException => (StatusCodes.Status422UnprocessableEntity, "Unprocessable Entity", "Data is misaligned."),

            // 501 Not Implemented
            NotImplementedException => (StatusCodes.Status501NotImplemented, "Not Implemented", "This feature is not implemented yet."),

            // 504 Gateway Timeout
            TimeoutException => (StatusCodes.Status504GatewayTimeout, "Gateway Timeout", "Operation timed out."),
            TaskCanceledException => (StatusCodes.Status504GatewayTimeout, "Gateway Timeout", "Task was canceled."),

            // 500 Internal Server Error (Database)
            DbUpdateException => (StatusCodes.Status500InternalServerError, "Internal Server Error", "Database update error (e.g., foreign key constraint violation)."),
            DbException => (StatusCodes.Status500InternalServerError, "Internal Server Error", "Database connection error, timeout, or authorization issues."),
            DataException => (StatusCodes.Status500InternalServerError, "Internal Server Error", "General database error."),
            TransactionException => (StatusCodes.Status500InternalServerError, "Internal Server Error", "Transaction error occurred."),

            // 500 Internal Server Error (System)
            StackOverflowException => (StatusCodes.Status500InternalServerError, "Internal Server Error", "Stack overflow error."),
            OutOfMemoryException => (StatusCodes.Status500InternalServerError, "Internal Server Error", "Out of memory."),
            IOException => (StatusCodes.Status500InternalServerError, "Internal Server Error", "IO error occurred."),
            ApplicationException => (StatusCodes.Status500InternalServerError, "Internal Server Error", "Application error occurred."),

            // Default
            _ => (StatusCodes.Status500InternalServerError, "Internal Server Error", "An unexpected error occurred.")
        };
    }
}

/// <summary>
/// Extension methods for exception handler middleware registration
/// </summary>
public static class ExceptionHandlerMiddlewareExtensions
{
    /// <summary>
    /// Adds the exception handler middleware services to the DI container.
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configureOptions">Optional action to configure exception handler options</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddCustomExceptionHandler(
        this IServiceCollection services,
        Action<ExceptionHandlerOptions>? configureOptions = null)
    {
        var options = new ExceptionHandlerOptions();
        configureOptions?.Invoke(options);

        services.AddSingleton(options);
        services.AddScoped<ExceptionHandlerMiddleware>();

        return services;
    }

    /// <summary>
    /// Registers the custom exception handler middleware which will log the exceptions on the server
    /// and return a user-friendly json response to the client when unhandled exceptions occur.
    /// </summary>
    /// <param name="app">The application builder</param>
    /// <returns>The application builder for chaining</returns>
    public static IApplicationBuilder UseCustomExceptionHandler(this IApplicationBuilder app)
    {
        return app.UseMiddleware<ExceptionHandlerMiddleware>();
    }
}

