using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IskoopDemo.Shared.Application.DTOs
{
    public class PagingInfo
    {
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
        public bool HasNextPage { get; set; }
        public bool HasPreviousPage { get; set; }
        public int? NextPage { get; set; }
        public int? PreviousPage { get; set; }

        public PagingInfo() { }

        public PagingInfo(int currentPage, int pageSize, int totalCount)
        {
            CurrentPage = currentPage;
            PageSize = pageSize;
            TotalCount = totalCount;
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            HasNextPage = currentPage < TotalPages;
            HasPreviousPage = currentPage > 1;
            NextPage = HasNextPage ? currentPage + 1 : null;
            PreviousPage = HasPreviousPage ? currentPage - 1 : null;
        }

        public bool IsFirstPage => CurrentPage == 1;
        public bool IsLastPage => CurrentPage == TotalPages;
        public bool IsEmpty => TotalCount == 0;
        public int StartRecord => TotalCount == 0 ? 0 : (CurrentPage - 1) * PageSize + 1;
        public int EndRecord => Math.Min(CurrentPage * PageSize, TotalCount);
    }
}
