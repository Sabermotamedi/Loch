using System;

namespace Loch.Shared.Application.Services;
public interface ILogService<T>
{
    void LogError(Exception ex, string message, params object[] args);
    void LogInformation(string message, params object[] args);
    void LogInformation(Exception ex, string message, params object[] args);
    void LogExecutingTime(int milisecond);
}
