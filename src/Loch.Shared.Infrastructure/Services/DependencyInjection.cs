using Loch.Shared.Application.Services;
using Loch.Shared.Infrastructure.Services.Implementations;
using Microsoft.Extensions.DependencyInjection;

namespace Loch.Shared.Infrastructure.Services;

public static class DependencyInjection
{
    public static IServiceCollection AddCommonServices(this IServiceCollection services)
    {
        services.AddRestSharpService();
        services.AddSmsService();
        return services;
    }
    public static void AddRestSharpService(this IServiceCollection services)
    {
        services.AddScoped<IRestSharpService,RestSharpService>();
    }
    public static void AddSmsService(this IServiceCollection services)
    {
        services.AddScoped<ISmsService,SmsService>();
    }
}
