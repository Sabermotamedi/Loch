using System;
using System.Reflection;
using Loch.Shared.Application.Services;
using Loch.Shared.Logging.Attributes;
using Loch.Shared.Logging.Services.Implementation;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Sinks.Elasticsearch;

namespace Loch.Shared.Logging.Services;
public static class DependencyInjection
{
    public static IServiceCollection AddLogStash(this IServiceCollection services, IConfiguration configuration, bool isDevelopment)
    {
        if (isDevelopment)
        {
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .Enrich.WithClientIp()
                .Enrich.WithClientAgent()
                .WriteTo.Elasticsearch(ConfigureElasticSink(configuration, "Development"))
                .Enrich.WithProperty("Environment", "Development")
                .CreateLogger();
        }
        else
        {
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .Enrich.WithClientIp()
                .Enrich.WithClientAgent()
                .WriteTo.Elasticsearch(ConfigureElasticSink(configuration, "Production"))
                .Enrich.WithProperty("Environment", "Production")
                .CreateLogger();
        }

        services.AddSingleton(typeof(ILogService<>), typeof(LogService<>));

        return services;

    }

    public static void AddExecutingActionTimeFilter(this FilterCollection filters)
    {
        filters.Add(typeof(ActionExecutionTimeAttribute));
    }

    public static void AddExceptionFilter(this FilterCollection filters)
    {
        filters.Add(typeof(ActionExceptionAttribute));
    }

    private static ElasticsearchSinkOptions ConfigureElasticSink(IConfiguration configuration, string environment)
    {
        var connection = configuration["ElasticConfiguration:Uri"];
        return new ElasticsearchSinkOptions(new Uri(connection))
        {
            AutoRegisterTemplate = true,
            IndexFormat = $"{Assembly.GetExecutingAssembly().GetName().Name.ToLower().Replace(".", "-")}-{environment?.ToLower().Replace(".", "-")}-{DateTime.UtcNow:yyyy}"
        };
    }
}
