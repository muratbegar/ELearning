using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELearningIskoop.Courses.Application.DTOs.Common
{
    // Sayfalama için temel request DTO'su
    public record PagedRequest
    {
        // Sayfa numarası (varsayılan: 1)
        public int PageNumber { get; init; } = 1;

        // Sayfa boyutu (varsayılan: 20, max: 100)
        public int PageSize { get; init; } = 20;

        // Sayfa boyutunu kontrol eder
        public int ValidatedPageSize => Math.Clamp(PageSize, 1, 100);

        // Sayfa numarasını kontrol eder
        public int ValidatedPageNumber => Math.Max(PageNumber, 1);
    }
}
