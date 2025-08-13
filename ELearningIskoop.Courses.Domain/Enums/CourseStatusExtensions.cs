using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELearningIskoop.Courses.Domain.Enums
{
    public static class CourseStatusExtensions
    {
        public static string GetDescription(this CourseStatus status) => status switch
        {
            CourseStatus.Draft => "Taslak",
            CourseStatus.Published => "Yayında",
            CourseStatus.Unpublished => "Yayından Kaldırılmış",
            CourseStatus.Archived => "Arşivlenmiş",
            _ => status.ToString()
        };

        public static bool CanBeModified(this CourseStatus status) => status == CourseStatus.Draft;

        public static bool IsAvailableForEnrollment(this CourseStatus status) => status == CourseStatus.Published;
    }
}
