namespace WebFormManager.API.Middlewares;

public class JsonResponseMiddleware
{
    private readonly RequestDelegate _next;

    public JsonResponseMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        context.Response.ContentType = "application/json";
        await _next(context);
    }
}