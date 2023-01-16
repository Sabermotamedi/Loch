using Loch.Shared.Application.Services;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Loch.Shared.Logging.Attributes;
public class ActionExceptionAttribute : ExceptionFilterAttribute
{
    private readonly ILogService<ActionExceptionAttribute> _logger;
    public ActionExceptionAttribute(ILogService<ActionExceptionAttribute> logger)
    {
        _logger = logger;
    }

    public override void OnException(ExceptionContext filterContext)
    {
        _logger.LogError(filterContext.Exception, filterContext.Exception.Message, "General Exception");
    }
}
