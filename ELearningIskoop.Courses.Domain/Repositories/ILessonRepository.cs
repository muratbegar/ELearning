using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ELearningIskoop.BuildingBlocks.Domain;
using ELearningIskoop.Courses.Domain.Entities;
using ELearningIskoop.Shared.Domain.Enums;

namespace ELearningIskoop.Courses.Domain.Repositories
{
    public interface ILessonRepository : IRepository<Lesson>
    {
        //kursa göre dersleri getir
        Task<IEnumerable<Lesson>> GetByCourseId(int courseId,CancellationToken cancellationToken);

        //Ücretsiz önizleme derslerini getir
        Task<IEnumerable<Lesson>> GetFreePreviewLessonsAsync(int courseId, CancellationToken cancellationToken);

        //içerik tipine göre dersleri getir
        Task<IEnumerable<Lesson>> GetByContentTypeAsync(ContentType contentType, CancellationToken cancellationToken);
    }
}
