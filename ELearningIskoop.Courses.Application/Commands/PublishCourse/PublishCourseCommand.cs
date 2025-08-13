using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ELearningIskoop.BuildingBlocks.Application.CQRS;

namespace ELearningIskoop.Courses.Application.Commands.PublishCourse
{
    // kurs yayınlama komutu
    public record PublishCourseCommand : BaseCommand<PublishCourseResponse>
    {
        public int ObjectId { get; init; }
    }

    public record PublishCourseResponse
    {
        public int CourseId { get; init; }
        public string Title { get; init; } = string.Empty;
        public string Status { get; init; } = string.Empty;
        public DateTime PublishedAt { get; init; }
    }
}
