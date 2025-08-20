using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IskoopDemo.Shared.Application.DTOs
{
    public class PagedResponse<T> : BaseResponse<IEnumerable<T>>
    {
        public PagingInfo Paging { get; set; }

        public PagedResponse():base()
        {
            Paging = new PagingInfo();
        }


        public PagedResponse(IEnumerable<T> data, PagingInfo paging, string message = null) : base(data,true,message)
        {
            Paging = paging;
        }

        public static PagedResponse<T> Success(
            IEnumerable<T> data,
            int currentPage,
            int pageSize,
            int totalCount,
            string message = "Data retrieved successfully")
        {
            var paging = new PagingInfo(currentPage, pageSize, totalCount);
            return new PagedResponse<T>(data, paging, message);
        }

        public static PagedResponse<T> Success(
            IEnumerable<T> data,
            PagingInfo paging,
            string message = "Data retrieved successfully")
        {
            return new PagedResponse<T>(data, paging, message);
        }
        public static PagedResponse<T> Empty(int currentPage = 1, int pageSize = 10)
        {
            var paging = new PagingInfo(currentPage, pageSize, 0);
            return new PagedResponse<T>(Enumerable.Empty<T>(), paging, "No data found");
        }
    }
}
