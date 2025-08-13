using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ELearningIskoop.BuildingBlocks.Application.CQRS;
using ELearningIskoop.Shared.Domain.Enums;

namespace ELearningIskoop.Courses.Application.Commands.CreateCourse
{
    public record CreateCourseCommand : BaseCommand<CreateCourseResponse>
    {
        //Kurs başlığı
        public string Title { get; init; } = string.Empty;

        // Kurs açıklaması
        public string Description { get; init; } = string.Empty;

        // Eğitmen adı
        public string InstructorFirstName { get; init; } = string.Empty;

        // Eğitmen soyadı
        public string InstructorLastName { get; init; } = string.Empty;

        // Eğitmen email adresi
        public string InstructorEmail { get; init; } = string.Empty;

        //Kurs Fiyatı
        public decimal Price { get; init; }

        // Para birimi (TRY, USD, EUR, GBP)
        public string Currency { get; init; } = "TRY";

        // Kurs seviyesi
        public CourseLevel Level { get; init; } = CourseLevel.Beginner;

        // Maksimum öğrenci sayısı
        public int MaxStudents { get; init; } = 1000;

        //Kategori ID
        public List<int> CategoryIds { get; init; } = new List<int>();


        // Thumbnail URL'si (opsiyonel)
        public string? ThumbnailUrl { get; init; } = null;

        // Trailer video URL'si (opsiyonel)
        public string? TrailerVideoUrl { get; init; }
    }

    // Kurs oluşturma response'u
    public record CreateCourseResponse
    {
        public int CourseId { get; set; }
        public string Title{ get; set; }
        public string Status{ get; set; }
        public DateTime CreatedAt{ get; set; }
    }
}
