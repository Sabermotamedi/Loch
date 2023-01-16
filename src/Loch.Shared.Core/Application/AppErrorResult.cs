// -----------------------------------------------------------------------
// <copyright file="AppErrorResult.cs" company="Loch">
// Copyright (c) Loch. All rights reserved.  Developed with 🖤 in development department.
// </copyright>
// -----------------------------------------------------------------------

namespace Loch.Shared.Core.Application
{
    public class AppErrorResult
    {
        public AppErrorResult(string code, string message)
        {
            Code = code;
            Message = message;
        }

        public string Code { get; }
        public string Message { get; }
    }
}