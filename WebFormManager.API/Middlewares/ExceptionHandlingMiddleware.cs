using System.Net;
using System.Text.Json;
using Serilog;
using WebFormManager.Infrastructure.Exceptions;
using ValidationException = FluentValidation.ValidationException;

namespace WebFormManager.API.Middlewares;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IWebHostEnvironment _env;

    public ExceptionHandlingMiddleware(RequestDelegate next, IWebHostEnvironment env)
    {
        _next = next;
        _env = env;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            Log.Information("Handling request: {Method} {Path}", context.Request.Method, context.Request.Path);
            await _next(context);
        }
        catch (InvalidOperationException ex)
        {
            await HandleExceptionAsync(context, ex, HttpStatusCode.Conflict, "Data conflict.");
        }
        catch (FileStorageException ex)
        {
            await HandleExceptionAsync(context, ex, HttpStatusCode.InternalServerError, "File storage error.");
        }
        catch (JsonException ex)
        {
            await HandleExceptionAsync(context, ex, HttpStatusCode.InternalServerError, "Json error.");
        }
        catch (ValidationException ex)
        {
            var errors = ex.Errors.Select(e => new { e.PropertyName, e.ErrorMessage }).ToList();
            var response = new { Message = "Validation failed.", Errors = errors };
    
            Log.Warning("Validation failed on {Path}: {@Errors}", context.Request.Path, errors);
    
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex, HttpStatusCode.InternalServerError, "Internal server error.");
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception, HttpStatusCode statusCode,
        string message, IReadOnlyList<string>? errors = null)
    {
        context.Response.StatusCode = (int)statusCode;
        context.Response.ContentType = "application/json";

        var response = new
        {
            Message = message,
            Details = _env.IsDevelopment() ? exception.ToString() : exception.Message
        };

        Log.Error(exception, "An error has occurred: {Message}", exception.Message);

        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}