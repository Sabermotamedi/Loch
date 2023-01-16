// -----------------------------------------------------------------------
// <copyright file="ObjectExtensions.cs" company="Loch">
// Copyright (c) Loch. All rights reserved.  Developed with 🖤 in development department.
// </copyright>
// -----------------------------------------------------------------------

namespace Loch.Shared.Extensions
{
    public static class ObjectExtensions
    {
        public static T Mutate<T>(this T @this, Action<T> action)
        {
            action?.Invoke(@this);

            return @this;
        }

        public static bool IsNull<T>(this T @this)
        {
            return @this == null;
        }

        public static bool IsNotNull<T>(this T @this)
        {
            return !@this.IsNull();
        }
    }
}