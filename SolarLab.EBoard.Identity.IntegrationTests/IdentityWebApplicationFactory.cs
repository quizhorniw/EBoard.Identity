using System.Data.Common;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SolarLab.EBoard.Identity.Application.Abstractions.Messaging;
using SolarLab.EBoard.Identity.Infrastructure.Persistence;
using SolarLab.EBoard.Identity.IntegrationTests.Helpers;

namespace SolarLab.EBoard.Identity.IntegrationTests;

public class IdentityWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            WebApplicationFactoryHelper.RemoveDbContext<AppDbContext>(services);
            
            services.AddSingleton<DbConnection>(_ =>
            {
                var connection = new SqliteConnection("DataSource=:memory:");
                connection.Open();

                return connection;
            });

            services.AddAuthentication("TestScheme")
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("TestScheme", opts => { });

            services.AddDbContext<AppDbContext>((container, options) =>
            {
                var connection = container.GetRequiredService<DbConnection>();
                options.UseSqlite(connection);
            });

            services.AddHostedService<DatabaseInitializerHostedService>();
            
            services.AddSingleton<IMessageProducer, NoOpMessageProducer>();
        });

        builder.ConfigureLogging(l => l.AddConsole().SetMinimumLevel(LogLevel.Debug));
        
        builder.UseEnvironment("IntegrationTests");
    }
}