using System.Net;
using System.Text;
using Confluent.Kafka;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using SolarLab.EBoard.Identity.Application.Abstractions.Authentication;
using SolarLab.EBoard.Identity.Application.Abstractions.Messaging;
using SolarLab.EBoard.Identity.Application.Abstractions.Persistence;
using SolarLab.EBoard.Identity.Application.Abstractions.Time;
using SolarLab.EBoard.Identity.Infrastructure.Authentication;
using SolarLab.EBoard.Identity.Infrastructure.ExceptionHandlers;
using SolarLab.EBoard.Identity.Infrastructure.Messaging;
using SolarLab.EBoard.Identity.Infrastructure.Persistence;
using SolarLab.EBoard.Identity.Infrastructure.Time;

namespace SolarLab.EBoard.Identity.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        return services
            .AddServices()
            .AddKafka()
            .AddDatabase()
            .AddAuthenticationInternal();
    }

    private static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
        
        services.AddExceptionHandler<BadRequestExceptionHandler>();
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();
        
        return services;
    }

    private static IServiceCollection AddKafka(this IServiceCollection services)
    {
        var kafkaConfig = new ProducerConfig
        {
            BootstrapServers = Environment.GetEnvironmentVariable("KAFKA_BOOTSTRAP_SERVER"),
            ClientId = Dns.GetHostName(),
        };
        services.AddSingleton<IProducer<string, string>>(_ => new ProducerBuilder<string, string>(kafkaConfig).Build());
        
        services.AddSingleton<IMessageProducer, KafkaNotificationProducer>();

        return services;
    }
    
    private static IServiceCollection AddDatabase(this IServiceCollection services)
    {
        services.AddDbContext<AppDbContext>(opts => opts
            .UseNpgsql(Environment.GetEnvironmentVariable("POSTGRES_CONNECTION_STRING"))
            .UseSnakeCaseNamingConvention());

        services.AddScoped<IUsersRepository, UsersRepository>();
        services.AddScoped<IRefreshTokensRepository, RefreshTokensRepository>();
        
        return services;
    }

    private static IServiceCollection AddAuthenticationInternal(this IServiceCollection services)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(o =>
            {
                o.RequireHttpsMetadata = false;
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                        Environment.GetEnvironmentVariable("JWT_SECRET")!)),
                    ValidIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER"),
                    ValidAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE"),
                    ClockSkew = TimeSpan.Zero
                };
            });

        services.AddAuthorization();
        services.AddHttpContextAccessor();
        services.AddScoped<IUserContext, UserContext>();
        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        services.AddSingleton<ITokenProvider, TokenProvider>();
        services.AddScoped<ICookieContext, CookieContext>();

        return services;
    }
}