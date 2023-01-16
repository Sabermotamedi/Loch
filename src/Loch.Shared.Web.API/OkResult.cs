// -----------------------------------------------------------------------
// <copyright file="OkResult.cs" company="Loch">
// Copyright (c) Loch. All rights reserved.  Developed with 🖤 in development department.
// </copyright>
// -----------------------------------------------------------------------

using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Loch.Shared.Core.Application;

namespace Loch.Shared.Web.API.Controllers
{
    public class OkResult : IActionResult
    {
        private readonly AppResult _result;
        public OkResult(AppResult result) => _result = result;

        public async Task ExecuteResultAsync(ActionContext context)
        {
            if (_result.IsSuccess && _result.Data != null)
            {
                var objectResult = new ContentResult
                {
                    StatusCode = StatusCodes.Status200OK,
                    Content = _result.Data as string,
                    ContentType = "application/json"
                };
                await objectResult.ExecuteResultAsync(context);
            }

            if (_result.IsSuccess && _result.Data == null)
            {
                context.HttpContext.Response.StatusCode = StatusCodes.Status404NotFound;
                await Task.FromResult(context);
            }

            if (!_result.IsSuccess)
            {
                var objectResult1 = new ObjectResult(_result.Errors)
                {
                    StatusCode = StatusCodes.Status400BadRequest
                };
                await objectResult1.ExecuteResultAsync(context);
            }
        }
    }
}
