// -----------------------------------------------------------------------
// <copyright file="PagedResultEFCoreExtensions.cs" company="Loch">
// Copyright (c) Loch. All rights reserved.  Developed with 🖤 in development department.
// </copyright>
// -----------------------------------------------------------------------

using Loch.Shared.Pagination;
using Microsoft.EntityFrameworkCore;

namespace Loch.Shared.EF.Extensions
{
    public static class PagedResultEFCoreExtensions
    {
        public static PagedResult<T> GetPaged<T>(this IQueryable<T> query, int page, int pageSize)
        {
            var result = new PagedResult<T>
            {
                PageIndex = page,
                PageSize = pageSize,
                Total = query.Count()
            };

            var pageCount = (double)result.Total / pageSize;
            result.PageCount = (int)Math.Ceiling(pageCount);

            var skip = (page - 1) * pageSize;
            result.Results = query.Skip(skip).Take(pageSize).ToList();

            return result;
        }

        public static async Task<PagedResult<T>> GetPagedAsync<T>(this IQueryable<T> query, int page, int pageSize)
        {
            var result = new PagedResult<T>
            {
                PageIndex = page,
                PageSize = pageSize,
                Total = await query.CountAsync()
            };

            var pageCount = (double)result.Total / pageSize;
            result.PageCount = (int)Math.Ceiling(pageCount);

            var skip = (page - 1) * pageSize;
            result.Results = await query.Skip(skip).Take(pageSize).ToListAsync();

            return result;
        }
    }
}