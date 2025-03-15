using System.Text.Encodings.Web;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.OpenApi.Models;
using Serilog;
using WebFormManager.API.Interfaces;
using WebFormManager.API.Middlewares;
using WebFormManager.API.Validation;
using WebFormManager.Infrastructure;

namespace WebFormManager.API;

public class Startup
{
    public IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "WebFormManager API",
                Version = "v1",
                Description = "WebForm management application"
            });
        });

        services.Configure<IISServerOptions>(options =>
        {
            options.MaxRequestBodySize = 1 * 1024 * 1024;
        });

        services.Configure<KestrelServerOptions>(options =>
        {
            options.Limits.MaxRequestBodySize = 1 * 1024 * 1024;
        });

        services.Configure<JsonOptions>(options =>
        {
            options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            options.JsonSerializerOptions.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
        });
        
        services.AddPersistenceServices(Configuration);
        
        services.AddScoped<ISubmissionValidator, SubmissionValidator>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseMiddleware<ExceptionHandlingMiddleware>();
        app.UseMiddleware<JsonResponseMiddleware>();
        app.UseMiddleware<RequestSizeLimitMiddleware>();
        
        app.UseHttpsRedirection();

        app.UseSerilogRequestLogging();
        
        app.UseRouting();
        
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
        
        Log.Information("Application started successfully");
    }
}