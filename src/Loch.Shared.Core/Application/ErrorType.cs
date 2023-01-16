// -----------------------------------------------------------------------
// <copyright file="ErrorType.cs" company="Loch">
// Copyright (c) Loch. All rights reserved.  Developed with 🖤 in development department.
// </copyright>
// -----------------------------------------------------------------------

using Loch.Shared.Types;

namespace Loch.Shared.Core.Application
{
    public enum Classification
    {
        Generic = DomainType.SystemManage,
    }

    // *********************************************************
    // Update wiki, if you update or add any error type.
    //
    // http://repo.Loch.com:3000/Loch
    //
    // ********************************************************
    public enum GenericErrorType
    {
        EntityRestrictDelete = Classification.Generic + 1,
        InvalidRequest = Classification.Generic + 2,
    }
}
