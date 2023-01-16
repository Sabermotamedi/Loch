// -----------------------------------------------------------------------
// <copyright file="Error.cs" company="Loch">
// Copyright (c) Loch. All rights reserved.  Developed with 🖤 in development department.
// </copyright>
// -----------------------------------------------------------------------

using System;
using Loch.Shared.Core.Application;

namespace Loch.Shared.Core.Domain
{
    public class Error
    {
        public long ErrorCode { get; }

        public Error(long errorCode)
        {
            ErrorCode = errorCode;
        }
    }
}
