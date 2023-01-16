using Loch.Shared.Core.Application;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Loch.Shared.Web.API.Controllers
{
    public class DeletedResult : ActionResult
    {
        private readonly AppResult _result;

        public DeletedResult(AppResult result) => _result = result;

        public async override Task ExecuteResultAsync(ActionContext context)
        {
            if (_result.IsSuccess)
            {
                context.HttpContext.Response.StatusCode = StatusCodes.Status204NoContent;
                await Task.FromResult(context);
            }
            else
            {
                var objectResult = new ObjectResult(_result.Errors)
                {
                    StatusCode = StatusCodes.Status422UnprocessableEntity
                };
                await objectResult.ExecuteResultAsync(context);
            }
        }
    }
}
