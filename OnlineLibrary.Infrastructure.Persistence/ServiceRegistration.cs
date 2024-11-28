using Microsoft.Extensions.DependencyInjection;
using OnlineLibrary.Infrastructure.Persistence.Contexts;

namespace OnlineLibrary.Infrastructure.Persistence;

public static class ServiceRegistration
{
    public static void AddPersistenceLayer(this IServiceCollection services, string connectionString)
    {
        services.AddSqlServer<AppDbContext>(connectionString, builder =>
        {
            builder.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName);
        });
    }
}