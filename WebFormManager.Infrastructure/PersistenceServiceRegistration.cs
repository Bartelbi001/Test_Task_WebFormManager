using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using WebFormManager.Application.Contracts.Persistence;
using WebFormManager.Infrastructure.Persistence;

namespace WebFormManager.Infrastructure;

public static class PersistenceServiceRegistration
{
    public static void AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
    {
        var storageType = configuration["Storage:Type"] ?? "InMemory";
        
        if (storageType == "File")
        {
            services.AddSingleton<ISubmissionStorage, FileSubmissionStorage>();
        }
        else
        {
            services.AddSingleton<ISubmissionStorage, InMemorySubmissionStorage>();
        }
        
        Log.Information($"Using {storageType} submission storage");
    }
}