using Microsoft.OpenApi.Models;

namespace SolarLab.EBoard.Identity.WebApi.Extensions;

public static class SwaggerExtensions
{
    public static IServiceCollection ConfigureSwaggerGen(this IServiceCollection services)
    {
        services.AddSwaggerGen(opts =>
        {
            var contactUrl = Environment.GetEnvironmentVariable("CONTACT_URL");
            opts.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Posts Service API",
                Version = "v1",
                Description = "An API to perform operations with posts, categories & comments",
                Contact = new OpenApiContact
                {
                    Name = Environment.GetEnvironmentVariable("CONTACT_NAME"),
                    Email = Environment.GetEnvironmentVariable("CONTACT_EMAIL"),
                    Url = contactUrl != null ? new Uri(contactUrl) : null
                }
            });
            
            opts.EnableAnnotations();
        });
        
        return services;
    }
}