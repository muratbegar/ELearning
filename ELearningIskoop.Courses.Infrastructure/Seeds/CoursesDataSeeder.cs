using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.Marshalling;
using System.Text;
using System.Threading.Tasks;
using ELearningIskoop.Courses.Domain.Entities;
using ELearningIskoop.Courses.Infrastructure.Persistence;
using ELearningIskoop.Shared.Domain.Enums;
using ELearningIskoop.Shared.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ELearningIskoop.Courses.Infrastructure.Seeds
{
    // Development ortamında sample data oluşturmak için seeder
    public static class CoursesDataSeeder
    {
        // Sample kategoriler ve kurslar oluşturur
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<CoursesDbContext>();
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<CoursesDbContext>>();

                try
                {
                    // Eğer veri varsa seeding yapma
                    if (await context.categories.IgnoreQueryFilters().AnyAsync())
                    {
                        logger.LogInformation("Database already seeded. Skipping seeding process.");
                        return;
                    }

                    logger.LogInformation("Starting data seeding...");

                    //Kategorileri Oluştur
                    var categories = CreateSampleCategories();
                    await context.categories.AddRangeAsync(categories);
                    await context.SaveChangesAsync();

                    //Kursları Oluştur
                    var courses = CreateSampleCourses(categories);
                    await context.Courses.AddRangeAsync(courses);
                    await context.SaveChangesAsync();

                    // kurs kategorilerini ilişkilendir
                    CreateCourseCategories(courses,categories,context);
                    await context.SaveChangesAsync();

                    //Dersleri Oluştur
                    CreateSampleLessons(courses, context);
                    await context.SaveChangesAsync();


                    logger.LogInformation("Data seeding completed successfully.");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "An error occurred while seeding data");
                    throw;
                }
            }
        }

        #region Yardımcı Methodlar

        private static List<Category> CreateSampleCategories()
        {
            return new List<Category>
            {
               Category.Create("Yazılım Geliştirme","Yazılım geliştime ile ilgili kurslar",1),//1 admin
               Category.Create("Web Tasarım", "Web tasarım ve UI/UX kursları",1),
               Category.Create("Mobil Geliştirme", "Mobil uygulama geliştirme kursları", 1),
               Category.Create("Veri Bilimi", "Veri analizi ve makine öğrenmesi kursları", 1),
               Category.Create("Siber Güvenlik", "Bilgi güvenliği ve siber güvenlik kursları", 1)
            };
        }

        private static List<Course> CreateSampleCourses(List<Category> categories)
        {
            return new List<Course>
            {
                Course.Create(
                    "C# ile Web Development",
                    "Sıfırdan ileri seviye C# ve ASP.NET Core ile web geliştirme",
                    PersonName.Create("Ahmet", "Yılmaz"),
                    Email.Create("ahmet@example.com"),
                    Money.CreateTRY(299.99m),
                    CourseLevel.Intermediate,
                    1 //admin
                ),

                Course.Create(
                    "JavaScript Temelleri",
                    "Modern JavaScript ile frontend geliştirme temelleri",
                    PersonName.Create("Ayşe", "Kara"),
                    Email.Create("ayse@example.com"),
                    Money.CreateTRY(199.99m),
                    CourseLevel.Beginner,
                    1),

                Course.Create(
                    "React ile Modern Web Uygulamaları",
                    "React, Redux ve modern frontend teknolojileri",
                    PersonName.Create("Mehmet", "Demir"),
                    Email.Create("mehmet@example.com"),
                    Money.CreateTRY(399.99m),
                    CourseLevel.Advanced,
                    1),

                Course.Create(
                    "Python ile Veri Analizi",
                    "Pandas, NumPy ve Matplotlib ile veri analizi",
                    PersonName.Create("Fatma", "Özkan"),
                    Email.Create("fatma@example.com"),
                    Money.CreateTRY(249.99m),
                    CourseLevel.Intermediate,
                    1),

                Course.Create(
                    "Ücretsiz HTML/CSS Kursu",
                    "Web geliştirmeye giriş - HTML ve CSS temelleri",
                    PersonName.Create("Can", "Arslan"),
                    Email.Create("can@example.com"),
                    Money.CreateTRY(0m),
                    CourseLevel.Beginner,
                    1)

            };
        }

        private static void CreateCourseCategories(List<Course> courses, List<Category> categories,
            CoursesDbContext context)
        {
            // C# kursu - Yazılım Geliştirme
            courses[0].AddCategory(categories[0], 1);

            // JavaScript kursu - Yazılım Geliştirme
            courses[1].AddCategory(categories[0], 1);

            // React kursu - Yazılım Geliştirme ve Web Tasarım
            courses[2].AddCategory(categories[0], 1);
            courses[2].AddCategory(categories[1], 1);

            // Python kursu - Veri Bilimi
            courses[3].AddCategory(categories[3], 1);

            // HTML/CSS kursu - Web Tasarım
            courses[4].AddCategory(categories[1], 1);

        }

        private static void CreateSampleLessons(List<Course> courses, CoursesDbContext context)
        {
            // C# kursu için dersler
            var csharpCourse = courses[0];
            csharpCourse.AddLesson(
                "C# Temelleri",
                "C# diline giriş ve temel kavramlar",
                Duration.CreateFromHoursAndMinutes(1, 30),
                ContentType.Video,
                1,
                1);

            csharpCourse.AddLesson(
                "OOP Prensipleri",
                "Nesne yönelimli programlama prensipleri",
                Duration.CreateFromHoursAndMinutes(2, 0),
                ContentType.Video,
                2,
                1);

            // JavaScript kursu için dersler
            var jsCourse = courses[1];
            jsCourse.AddLesson(
                "JavaScript Giriş",
                "JavaScript temelleri ve DOM manipülasyonu",
                Duration.FromMinutes(90),
                ContentType.Video,
                1,
                1);

            // Ücretsiz dersi işaretle
            var firstLesson = jsCourse.Lessons.First();
            firstLesson.MarkAsFree(1);//1 admin

            // React kursu için dersler
            var reactCourse = courses[2];
            reactCourse.AddLesson(
                "React Bileşenleri",
                "React component'leri ve JSX",
                Duration.CreateFromHoursAndMinutes(1, 45),
                ContentType.Video,
                1,
                1);

            reactCourse.AddLesson(
                "State Yönetimi",
                "useState ve useEffect hook'ları",
                Duration.CreateFromHoursAndMinutes(2, 15),
                ContentType.Video,
                2,
                1);
        }

        #endregion
    }
}
