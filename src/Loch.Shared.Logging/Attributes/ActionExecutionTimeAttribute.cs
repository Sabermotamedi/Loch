using System;
using System.Linq;
using Loch.Shared.Application.Services;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Loch.Shared.Logging.Attributes;
public class ActionExecutionTimeAttribute : ActionFilterAttribute
{
    private readonly ILogService<ActionExecutionTimeAttribute> _logger;
    private const string StartTime = "StartTime";
    public ActionExecutionTimeAttribute(ILogService<ActionExecutionTimeAttribute> logger)
    {
        _logger = logger;
    }

    public override void OnActionExecuting(ActionExecutingContext filterContext)
    {
        filterContext.HttpContext.Items[StartTime] = DateTime.UtcNow;
        base.OnActionExecuting(filterContext);
    }
    // Do this to calcution of Action Start to Result Process
    public override void OnResultExecuted(ResultExecutedContext filterContext)
    {
        base.OnResultExecuted(filterContext);
        if (filterContext.HttpContext.Items.Any(c => c.Key == StartTime))
        {
            DateTime startTime = (DateTime)filterContext.HttpContext.Items[StartTime];
            var milisecond = (int)(DateTime.UtcNow - startTime).TotalMilliseconds;
            if (milisecond > 1000)
            {
                _logger.LogExecutingTime(milisecond);
            }
        }
    }
}
