using FluentValidation;
using Microsoft.OpenApi.Models;
using Serilog;
using WebFormManager.API.Middlewares;
using WebFormManager.Application.Validation;
using WebFormManager.Domain.Entities;
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
        // services.AddDbContext<ApplicationDbContext>(options => 
        //     options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection")));

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
        
        services.AddPersistenceServices(Configuration);
        
        services.AddValidatorsFromAssemblyContaining<FormSubmission>();
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