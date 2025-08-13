using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ELearningIskoop.BuildingBlocks.Application.CQRS;
using ELearningIskoop.Courses.Application.Commands.PublishCourse;
using ELearningIskoop.Shared.Domain.Enums;

namespace ELearningIskoop.Courses.Application.Commands.AddLesson
{
    public record AddLessonCommand : BaseCommand<AddLessonResponse>
    {
        // Kurs ID'si
        public int CourseId { get; init; }

        // Ders başlığı
        public string Title { get; init; } = string.Empty;

        //Ders Açıklaması
        public string Description { get; init; } = string.Empty;

        // Ders süresi (dakika)
        public int DurationMinutes { get; init; }

        // İçerik tipi
        public ContentType ContentType { get; init; }

        // Ders sırası
        public int Order { get; init; }

        // İçerik URL'si (video, doküman vs.)
        public string? ContentUrl { get; init; }

        // Ücretsiz önizleme mi?
        public bool IsFreePreview { get; init; } = false;
    }

    public record AddLessonResponse
    {
        public int LessonId { get; init; }
        public int CourseId { get; init; }
        public string Title { get; init; } = string.Empty;
        public int Order { get; init; }

        public string ContentType { get; init; } = string.Empty;
        public string Duration { get; init; } = string.Empty;
    }
}
