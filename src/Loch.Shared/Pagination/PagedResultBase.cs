using System;

namespace Loch.Shared.Pagination
{
    public abstract class PagedResultBase
    {
        public int PageIndex { get; set; }

        public int PageCount { get; set; }

        public int PageSize { get; set; }

        public int Total { get; set; }
        public string LinkTemplate { get; set; }

        public int FirstRowOnPage => (PageIndex - 1) * PageSize + 1;

        public int LastRowOnPage
        {
            get { return Math.Min(PageIndex * PageSize, Total); }
        }
    }
}