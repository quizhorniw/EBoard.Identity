using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace SolarLab.EBoard.Identity.IntegrationTests.Helpers;

public class WebApplicationFactoryHelper
{
    public static void RemoveDbContext<TContext>(IServiceCollection services) where TContext : DbContext
    {
        var dbContextDescriptor = services.SingleOrDefault(d => 
            d.ServiceType == typeof(IDbContextOptionsConfiguration<TContext>));
        if (dbContextDescriptor is not null)
        {
            services.Remove(dbContextDescriptor);
        }

        var dbConnectionDescriptor = services.SingleOrDefault(d => 
            d.ServiceType == typeof(DbConnection));
        if (dbConnectionDescriptor is not null)
        {
            services.Remove(dbConnectionDescriptor);
        }
    }
}