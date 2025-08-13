using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELearningIskoop.BuildingBlocks.Domain.Repositories
{
    //kurs istatistiklerini value object olarak tutar
    public class CourseStatistics : ValueObject
    {
        public int TotalCourses { get; init; }
        public int PublishedCourses { get; init; }
        public int DraftCourses { get; init; }
        public int TotalLessons { get; init; }
        public int TotalStudents { get; init; }
        public int TotalInstructors { get; init; }
        public int AverageRating { get; init; }
        public TimeSpan TotalContentDuration { get; init; }

        public CourseStatistics(int totalCourses,int publishedCourses,int draftCourses,int totalLessons,int totalStudents,int totalInstructors,decimal avarageRating,TimeSpan totalContentDuration)
        {
            TotalCourses = totalCourses;
            PublishedCourses = publishedCourses;
            DraftCourses = draftCourses;
            TotalLessons = totalLessons;
            TotalStudents = totalStudents;
            TotalInstructors = totalInstructors;
            AverageRating = (int)avarageRating;
            TotalContentDuration = totalContentDuration;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return TotalCourses;
            yield return PublishedCourses;
            yield return DraftCourses;
            yield return TotalLessons;
            yield return TotalStudents;
            yield return TotalInstructors;
            yield return AverageRating;
            yield return TotalContentDuration;
        }
    }
}
