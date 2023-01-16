using System;
using Loch.Shared.Application.Services;
using Microsoft.Extensions.Logging;

namespace Loch.Shared.Logging.Services.Implementation;
public class LogService<T> : ILogService<T>
{

    private readonly ILogger<T> _logger;

    private const string ExecutingTime = "{ellapsedTime}";
    public LogService(ILogger<T> logger)
    {
        _logger = logger;
    }

    public void LogError(Exception ex, string message, params object[] args)
    {
        _logger.LogError(ex, message + " {args}", args);
    }

    public void LogInformation(Exception ex, string message, params object[] args)
    {
        _logger.LogInformation(ex, message + " {args}", args);
    }

    public void LogInformation(string message, params object[] args)
    {
        _logger.LogInformation(message + " {args}", args);
    }

    public void LogExecutingTime(int milisecond)
    {
        _logger.LogInformation(ExecutingTime, milisecond);
    }
}
