using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Loch.Shared.Application;
public static class ApplicationServiceRegistration
{
    public static IServiceCollection AddCommonApplicationServices(this IServiceCollection services)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        return services;
    }
}
