using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using ELearningIskoop.Courses.Domain.Entities;
using ELearningIskoop.Courses.Domain.Enums;
using ELearningIskoop.Shared.Domain.Enums;

namespace ELearningIskoop.Courses.Domain.Specifications
{
    // Kurs specification'ları
    // Karmaşık sorgu logic'lerini encapsulate eder
    public static class CourseSpecifications
    {
        //Yayındaki kurslar
        public static Expression<Func<Course, bool>> Published =>  course => course.Status == CourseStatus.Published;
        //ücretsiz kurslar
        public static Expression<Func<Course, bool>> Free => course => course.Price.Amount == 0;
        // Belirli seviyedeki kurslar
        public static Expression<Func<Course, bool>> ByLevel(CourseLevel level)
        {
            return course => course.Level == level;
        }

        // Belirli eğitmene ait kurslar
        public static Expression<Func<Course, bool>> ByInstructor(string instructorEmail) =>
            course => course.InstructorEmail.Value == instructorEmail;

        // Kapasitesi dolu olmayan kurslar
        public static Expression<Func<Course, bool>> Available => course => course.Status == Enums.CourseStatus.Published && course.CurrentStudentCount < course.MaxStudents;

        // Minimum ders sayısına sahip kurslar
        public static Expression<Func<Course, bool>> HasMinimumLessons(int minimumLessonCount) => course => course.Lessons.Count >= minimumLessonCount;

        // Belirli fiyat aralığındaki kurslar
        public static Expression<Func<Course, bool>> InPriceRange(decimal minPrice, decimal maxPrice) =>  course => course.Price.Amount >= minPrice && course.Price.Amount <= maxPrice;

        // Başlık veya açıklamada arama
        public static Expression<Func<Course, bool>> SearchByText(string searchTerm) =>
            course => course.Title.Contains(searchTerm) ||
                      course.Description.Contains(searchTerm);
    }
}
