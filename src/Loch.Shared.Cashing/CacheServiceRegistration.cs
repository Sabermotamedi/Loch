using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Loch.Shared.Caching;
public static class CacheServiceRegistration
{
    public static IServiceCollection AddDistributedRedisCache(this IServiceCollection services,IConfiguration configuration)
    {
        services.AddStackExchangeRedisCache(options => {
            options.Configuration = configuration.GetConnectionString("RedisConnectionString");
            options.InstanceName = "Loch";
        });
        return services;
    }


}
