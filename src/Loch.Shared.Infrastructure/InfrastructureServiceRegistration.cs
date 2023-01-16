using Loch.Shared.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Loch.Shared.Infrastructure;
public static class InfrastructureServiceRegistration
{
    public static IServiceCollection AddCommonInfrastructureServices(this IServiceCollection services)
    {
        services.AddCommonServices();
        return services;
    }


}
