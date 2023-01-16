// -----------------------------------------------------------------------
// <copyright file="CreatedResult.cs" company="Loch">
// Copyright (c) Loch. All rights reserved.  Developed with 🖤 in development department.
// </copyright>
// -----------------------------------------------------------------------

using Loch.Shared.Core.Application;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Loch.Shared.Web.API.Controllers
{
    public class CreatedResult : IActionResult
    {
        private readonly AppResult _result;

        public CreatedResult(AppResult result) => _result = result;

        public async Task ExecuteResultAsync(ActionContext context)
        {
            if (_result.IsSuccess)
            {
                var objectResult = new ObjectResult(_result)
                {
                    StatusCode = StatusCodes.Status201Created,
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
