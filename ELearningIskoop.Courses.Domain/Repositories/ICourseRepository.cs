using ELearningIskoop.BuildingBlocks.Domain;
using ELearningIskoop.Courses.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ELearningIskoop.BuildingBlocks.Domain.Repositories;
using ELearningIskoop.Shared.Domain.Enums;


namespace ELearningIskoop.Courses.Domain.Repositories
{
    public interface ICourseRepository : IRepository<Course>
    {
        // Eğitmen ID'sine göre kursları getirir
        Task<IEnumerable<Course>> GetByInstructorEmailAsync(string instructorEmail,CancellationToken cancellationToken = default);

        // Seviyeye göre kursları getirir
        Task<IEnumerable<Course>> GetByLevelAsync(CourseLevel level, CancellationToken cancellationToken = default);

        // Kategoriye göre kursları getirir
        Task<IEnumerable<Course>> GetByCategoryAsync(int categoryId, CancellationToken cancellationToken = default);

        // Yayındaki kursları getirir
        Task<IEnumerable<Course>> GetPublishedCoursesAsync(int pageNumber = 1, int pageSize = 20, CancellationToken cancellationToken = default);

        // Ücretsiz kursları getirir
        Task<IEnumerable<Course>> GetFreeCoursesAsync(CancellationToken cancellationToken = default);

        // Kurs arama
        Task<IEnumerable<Course>> SearchAsync(string searchTerm,CourseLevel? level=null,int? categoryId=null,bool? isFree=null, 
            int pageNumber = 1, int pageSize = 20, CancellationToken cancellationToken = default);

        // Popüler kursları getirir (öğrenci sayısına göre)
        Task<IEnumerable<Course>> GetPopularCoursesAsync(int count = 10, CancellationToken cancellationToken = default);

        // Kurs istatistiklerini getirir
        Task<CourseStatistics> GetStatisticsAsync(int courseId, CancellationToken cancellationToken = default);

        // Slug'a göre kurs getirir
        Task<Course> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);



    }
}
