// -----------------------------------------------------------------------
// <copyright file="BaseController.cs" company="Loch">
// Copyright (c) Loch. All rights reserved.  Developed with 🖤 in development department.
// </copyright>
// -----------------------------------------------------------------------

using Loch.Shared.Core.Application;
using Microsoft.AspNetCore.Mvc;

namespace Loch.Shared.Web.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseController : ControllerBase
    {
        protected IActionResult Result(AppResult result)
             => new OkResult(result);

        protected IActionResult Updated(AppResult result)
             => new UpdatedResult(result);

        protected IActionResult Created(AppResult result)
             => new CreatedResult(result);

        protected IActionResult Deleted(AppResult result)
             => new DeletedResult(result);
    }
}
