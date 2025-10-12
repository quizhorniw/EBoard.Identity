using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SolarLab.EBoard.Identity.Infrastructure.Persistence;

namespace SolarLab.EBoard.Identity.IntegrationTests;

internal class DatabaseInitializerHostedService : IHostedService
{
    private readonly IServiceProvider _provider;

    public DatabaseInitializerHostedService(IServiceProvider provider)
    {
        _provider = provider;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _provider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await db.Database.EnsureCreatedAsync(cancellationToken);
    }
    
    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}