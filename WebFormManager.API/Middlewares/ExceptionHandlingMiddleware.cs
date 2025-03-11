using System.Net;
using System.Text.Json;
using Serilog;

namespace WebFormManager.API.Middlewares;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (InvalidOperationException exception)
        {
            await HandleExceptionAsync(context, exception, HttpStatusCode.Conflict, "Data conflict.");
        }
        catch (Exception exception)
        {
            await HandleExceptionAsync(context, exception, HttpStatusCode.InternalServerError, "Internal server error.");
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception exception, HttpStatusCode statusCode,
        string message)
    {
        context.Response.StatusCode = (int)statusCode;
        context.Response.ContentType = "application/json";

        var response = new
        {
            Message = message,
            Details = exception.Message
        };

        if (statusCode == HttpStatusCode.InternalServerError)
        {
            Log.Fatal(exception, "Fatal exception: {Message}", exception.Message);
        }
        else
        {
            Log.Error(exception, "An error has occurred: {Message}", exception.Message);
        }

        return context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}