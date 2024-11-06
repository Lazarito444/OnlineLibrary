using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace OnlineLibrary.Core.Application;

public static class ServiceRegistration
{
    public static void AddApplicationLayer(this IServiceCollection services)
    {
        // REGISTER AUTOMAPPER
        services.AddAutoMapper(Assembly.GetExecutingAssembly());

        // TODO: REGISTER SERVICES CLASSES
    }
}