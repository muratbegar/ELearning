using ELearningIskoop.BuildingBlocks.Application.CQRS;
using ELearningIskoop.Courses.Application.Queries.SearchCourses.Dto;
using ELearningIskoop.Shared.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELearningIskoop.Courses.Application.Queries.SearchCourses
{
    public record SearchCoursesQuery : BaseQuery<SearchCoursesResponse>
    {
        //Arama Terimi
        public string SearchTerm { get; init; } = string.Empty;

        //Sayfa Numarası
        public int PageNumber { get; init; } = 1;

        //Sayfa Boyutu
        public int PageSize { get; init; } = 20;

        //Kategori Filterleri
        public int CategoryId { get; init; }

        //Seviye Filterleri
        public CourseLevel? Level { get; init; }

        //ücretsiz filtresi
        public bool? IsFree { get; init; }

        //Mininmum Fiyat
        public decimal? MinPrice { get; init; }

        //Maksimum Fiyat
        public decimal? MaxPrice { get; init; }
    }
}
