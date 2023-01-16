using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Loch.Shared.EF.Extensions
{
    public static class EntityFrameworkExtensions
    {
        public static IQueryable<TSource> WhereIf<TSource>(this IQueryable<TSource> source, bool condition, Expression<Func<TSource, bool>> predicate)
        {
            if (condition)
            {
                return source.Where(predicate);
            }
            else
            {
                return source;
            }
        }

        public static IEnumerable<TSource> WhereIf<TSource>(this IEnumerable<TSource> source, bool condition, Func<TSource, bool> predicate)
        {
            if (condition)
            {
                return source.Where(predicate);
            }
            else
            {
                return source;
            }
        }

        public static IEnumerable<TSource> WhereIf<TSource>(this IEnumerable<TSource> source, bool condition, Func<TSource, int, bool> predicate)
        {
            if (condition)
            {
                return source.Where(predicate);
            }
            else
            {
                return source;
            }
        }

        public static IEnumerable<T> DistinctBy<T, TKey>(this IEnumerable<T> items, Func<T, TKey> property)
        {
            return items.GroupBy(property).Select(x => x.First());
        }

        public static IEnumerable<T> Flatten<T>(this IEnumerable<T> source, Func<T, IEnumerable<T>> childrenSelector)
        {
            // Do standard error checking here.

            // Cycle through all of the items.
            foreach (T item in source)
            {
                // Yield the item.
                yield return item;
                if (childrenSelector(item)?.Any() != true)
                {
                    continue;
                }

                // Yield all of the children.
                foreach (var child in childrenSelector(item).Flatten(childrenSelector))
                {
                    // Yield the item.
                    yield return child;
                }
            }
        }

        public static IEnumerable<T> SelectManyRecursive<T>(this IEnumerable<T> source, Func<T, IEnumerable<T>> selector)
        {
            var result = source.SelectMany(selector);
            if (!result.Any())
            {
                return result;
            }

            return result.Concat(result.SelectManyRecursive(selector));
        }
    }
}
