using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Loch.Shared.Core.Application;

namespace Loch.Shared.Web.API.Controllers
{
    public class UpdatedResult : IActionResult
    {
        private readonly AppResult _result;
        public UpdatedResult(AppResult result) => _result = result;

        public async Task ExecuteResultAsync(ActionContext context)
        {
            if (_result.IsSuccess)
            {
                var objectResult = new ObjectResult(_result.Data)
                {
                    StatusCode = StatusCodes.Status200OK,
                };
                await objectResult.ExecuteResultAsync(context);
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
