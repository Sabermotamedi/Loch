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
