// -----------------------------------------------------------------------
// <copyright file="ServiceProviderExtensions.cs" company="Loch">
// Copyright (c) Loch. All rights reserved.  Developed with 🖤 in development department.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace Loch.Shared.Extensions
{
    public static class ServiceProviderExtensions
    {
        public static T ResolveService<T>(this IServiceProvider provider)
            where T : class
        {
            if (provider == null)
            {
                throw new ArgumentNullException(nameof(provider));
            }

            return (T)provider.GetService(typeof(T));
        }
    }
}