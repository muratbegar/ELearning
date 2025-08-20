using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IskoopDemo.Shared.Application.DTOs
{
    public class ListResponse<T> : BaseResponse<IEnumerable<T>>
    {
        public int Count { get; set; }

        public ListResponse() : base() { }

        public ListResponse(IEnumerable<T> data, string message = null) : base(data, true, message)
        {
            Count = data?.Count() ?? 0;
        }

        public static ListResponse<T> Success(IEnumerable<T> data, string message = "Data retrieved successfully")
        {
            return new ListResponse<T>(data, message);
        }

        public static ListResponse<T> Empty(string message = "No data found")
        {
            return new ListResponse<T>(Enumerable.Empty<T>(), message);
        }
    }
}
