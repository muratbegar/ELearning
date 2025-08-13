using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ELearningIskoop.Courses.Domain.Repositories;

namespace ELearningIskoop.Courses.Domain.Services
{
    // Kurs domain service'i
    public class CourseDomainService
    {
        private readonly ICourseRepository _courseRepository;
        private readonly ICategoryRepository _categoryRepository;

        public CourseDomainService(ICourseRepository courseRepository, ICategoryRepository categoryRepository)
        {
            _courseRepository = courseRepository ?? throw new ArgumentNullException(nameof(courseRepository));
            _categoryRepository = categoryRepository?? throw new ArgumentNullException(nameof(categoryRepository));
        }

        // Kurs slug'ının unique olup olmadığını kontrol eder
        public async Task<bool> IsSlugUniqueAsync(string slug, int? excludeCourseId = null)
        {
            var existingCourse = await _courseRepository.GetBySlugAsync(slug);

            if (existingCourse == null)
            {
                return true; // Slug unique
            }

            //Güncelleme drumu için kontrol
            return excludeCourseId.HasValue && existingCourse.ObjectId == excludeCourseId.Value
                ? true // Güncellenen kursun slug'ı ile aynı ise unique sayılır
                : false; // Başka bir kurs ile çakışıyor
        }


        // Kategori slug'ının unique olup olmadığını kontrol eder
        public async Task<bool> IsCategorySlugUniqueAsync(string slug, int? excludeCategoryId = null)
        {
            var existingCategory = await _courseRepository.GetBySlugAsync(slug);
            if (existingCategory == null)
            {
                return true; // Slug unique
            }

            //Güncelleme durumu için kontrol
            return excludeCategoryId.HasValue && existingCategory.ObjectId == excludeCategoryId.Value
                ? true // Güncellenen kategorinin slug'ı ile aynı ise unique sayılır
                : false; // Başka bir kategori ile çakışıyor
        }

        // Eğitmenin kurs oluşturma limitini kontrol eder
        public async Task<bool> CanInstructorCreateMoreCourseAsync(string instructorEmail,
            int maxCoursesPerInstructor = 50)
        {
            var instructorCourses = await _courseRepository.GetByInstructorEmailAsync(instructorEmail);
            return instructorCourses.Count() < maxCoursesPerInstructor;
        }

        // Kategori silinebilir mi kontrol eder (alt kategorileri ve kursları var mı?)
        public async Task<bool> CanCategoryBeDeletedAsync(int categoryId)
        {
            var category = await _courseRepository.GetByIdAsync(categoryId);
            if (category == null)
            {
                return false; // Kategori bulunamadı
            }
            // Kategorinin alt kategorileri var mı?
            var subCategories = await _categoryRepository.GetSubCategoriesAsync(categoryId);
            if (subCategories.Any())
                return false;

            // Kategorinin kursları var mı?
            var coursesInCategory = await _courseRepository.GetByCategoryAsync(categoryId);
            return !coursesInCategory.Any();
        }

        // Kurs duplicate kontrolü yapar (başlık ve eğitmen bazında)
        public async Task<bool> IsCourseUniqueAsync(string title, string instructorEmail, int? excludeCourseId = null)
        {
            var instructorCourses = await _courseRepository.GetByInstructorEmailAsync(instructorEmail);
            var duplicateCourses =
                instructorCourses.FirstOrDefault(x => x.Title.Equals(title, StringComparison.OrdinalIgnoreCase));
            if (duplicateCourses == null)
                return true; // Kurs unique
            
            return excludeCourseId.HasValue && duplicateCourses.ObjectId == excludeCourseId.Value
                ? true // Güncellenen kurs ile aynı ise unique sayılır
                : false; // Başka bir kurs ile çakışıyor
        }
    }
}
