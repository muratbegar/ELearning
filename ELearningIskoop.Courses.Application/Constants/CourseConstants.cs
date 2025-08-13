using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELearningIskoop.Courses.Application.Constants
{
    public static class CourseConstants
    {
        // Validation sabitleri
        public static class Validation
        {
            public const int MaxTitleLength = 200;
            public const int MaxDescriptionLength = 2000;
            public const int MaxInstructorNameLength = 50;
            public const int MinInstructorNameLength = 2;
            public const int MaxStudentsLimit = 10000;
            public const int MaxCategoriesPerCourse = 5;
            public const int MaxLessonDurationMinutes = 10080; // 1 hafta
        }

        // Business rule sabitleri
        public static class BusinessRules
        {
            public const int MaxCoursesPerInstructor = 50;
            public const int MinLessonsToPublish = 1;
            public const int DefaultMaxStudents = 1000;
        }

        // Cache sabitleri
        public static class Cache
        {
            public const string CourseKeyPrefix = "course:";
            public const string CoursesListKeyPrefix = "courses:";
            public const int CourseDetailCacheMinutes = 15;
            public const int CoursesListCacheMinutes = 5;
        }

        // Pagination sabitleri
        public static class Pagination
        {
            public const int DefaultPageSize = 20;
            public const int MaxPageSize = 100;
            public const int MinPageSize = 1;
        }
    }
}
