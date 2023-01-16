﻿// -----------------------------------------------------------------------
// <copyright file="IUnitOfWork.cs" company="Loch">
// Copyright (c) Loch. All rights reserved.  Developed with 🖤 in development department.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Loch.Shared.Core.Data
{
    public interface IUnitOfWork : IDisposable
    {
        Task<bool> CommitAsync(CancellationToken cancellationToken = default);
        bool Commit(CancellationToken cancellationToken = default);
    }
}