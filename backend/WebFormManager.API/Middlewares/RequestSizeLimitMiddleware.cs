using System.Text.Json;
using Serilog;

namespace WebFormManager.API.Middlewares;

public class RequestSizeLimitMiddleware
{
    private readonly RequestDelegate _next;

    public RequestSizeLimitMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (BadHttpRequestException ex) when (ex.StatusCode == StatusCodes.Status413PayloadTooLarge)
        {
            Log.Warning("Request too large: {Path}", context.Request.Path);
            
            context.Response.StatusCode = StatusCodes.Status413PayloadTooLarge;
            context.Response.ContentType = "application/json";
            
            var response = new { message = "Request payload is too large. Maximum allowed size is 512KB." };
            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}