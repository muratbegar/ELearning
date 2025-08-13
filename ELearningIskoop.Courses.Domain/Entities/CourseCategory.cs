using ELearningIskoop.BuildingBlocks.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELearningIskoop.Courses.Domain.Entities
{
    // Kurs-Kategori ilişki entity'si (Many-to-Many)
    public class CourseCategory : BaseEntity
    {
        protected CourseCategory() { } // EF Core için

        private CourseCategory(int courseId, int categoryId)
        {
            CourseId = courseId;
            CategoryId = categoryId;
            CreatedAt = DateTime.UtcNow;
        }

        // Properties
        public int CourseId { get; private set; }
        public int CategoryId { get; private set; }

        // Navigation Properties
        public Course Course { get; private set; } = null!;
        public Category Category { get; private set; } = null!;

        /// <summary>
        /// Yeni kurs-kategori ilişkisi oluşturucu metod
        /// </summary>
        public static CourseCategory Create(int courseId, int categoryId)
        {
            return new CourseCategory(courseId, categoryId);
        }
    }
}
