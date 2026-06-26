using Orders.Domain.Exceptions;
using System.Net;
using System.Text.Json;
using FluentValidation;


namespace Orders.API.Middlewares;

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
            _logger.LogError(ex, "Unhandled exception: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, title, errors) = exception switch
        {
            ValidationException ve => (HttpStatusCode.BadRequest, "Validation Error", ve.Errors.Select(e => e.ErrorMessage).ToArray()),
            OrderNotFoundException => (HttpStatusCode.NotFound, exception.Message, Array.Empty<string>()),
            ProductNotFoundException => (HttpStatusCode.UnprocessableEntity, exception.Message, Array.Empty<string>()),
            InsufficientStockException => (HttpStatusCode.UnprocessableEntity, exception.Message, Array.Empty<string>()),
            InvalidOrderOperationException => (HttpStatusCode.UnprocessableEntity, exception.Message, Array.Empty<string>()),
            DomainException => (HttpStatusCode.BadRequest, exception.Message, Array.Empty<string>()),
            ArgumentException => (HttpStatusCode.BadRequest, exception.Message, Array.Empty<string>()),
            _ => (HttpStatusCode.InternalServerError, "An unexpected error occurred.", Array.Empty<string>())
        };

        context.Response.StatusCode = (int)statusCode;
        context.Response.ContentType = "application/json";

        var response = new
        {
            status = (int)statusCode,
            title,
            errors = errors.Length > 0 ? errors : null
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response,
            new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }));
    }
}
