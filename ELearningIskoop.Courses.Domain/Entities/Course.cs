using ELearningIskoop.BuildingBlocks.Domain;
using ELearningIskoop.Shared.Domain.Enums;
using ELearningIskoop.Shared.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ELearningIskoop.Courses.Domain.Enums;
using ELearningIskoop.Courses.Domain.Events;

namespace ELearningIskoop.Courses.Domain.Entities
{
    // Kurs entity'si - Aggregate Root
    // Bu sınıf, kurs ile ilgili temel bilgileri ve business kurallarını yönetir
    public class Course : BaseEntity
    {
        private readonly List<Lesson> _lessons = new();
        private readonly List<CourseCategory> _categories = new();


        protected Course() { } // EF Core için

        private Course(string title, string description, PersonName instructorName, Email instructorEmail, Money price,
            CourseLevel level)
        {
            Title = title;
            Description = description;
            InstructorName = instructorName;
            InstructorEmail = instructorEmail;
            Price = price;
            Level = level;
            Status = CourseStatus.Draft;
            CreatedAt = DateTime.UtcNow;
        }


        //Properties
        public string Title { get; private set; } = string.Empty;
        public string Description { get; private set; } = string.Empty;
        public PersonName InstructorName { get; private set; } = null;

        public Email InstructorEmail { get; private set; } = null;
        public Money Price { get; private set; } = null;
        public CourseLevel Level { get; private set; }
        public CourseStatus Status { get; private set; } 

        public DateTime? PublishedAt { get; private set; }

        public Duration? TotalDuration { get; private set; }

        public int MaxStudents { get; private set; } = 1000;
        public int CurrentStudentCount { get; private set; } = 0;

        public string? ThumbnailUrl { get; private set; } = null;
        public string? TrailerVideoUrl { get; private set; } = null;

        // Navigation Properties
        public IReadOnlyCollection<Lesson> Lessons => _lessons.AsReadOnly();
        public IReadOnlyCollection<CourseCategory> Categories => _categories.AsReadOnly();

        // Yeni kurs oluşturucu metod
        public static Course Create(string title, string description, PersonName instructorName, Email instructorEmail,
            Money price, CourseLevel level, int createdBy)
        {
            //business kurallarını kontrol et
            if(string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Kurs başlığı boş olamaz.", "COURSE_TITLE_REQUIRED");
            if(title.Length > 200)
                throw new ArgumentException("Kurs başlığı 100 karakterden uzun olamaz.", "COURSE_TITLE_TOO_LONG");
            if(string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("Kurs açıklaması boş olamaz.", "COURSE_DESCRIPTION_REQUIRED");
            if(description.Length > 2000)
                throw new ArgumentException("Kurs açıklaması 2000 karakterden uzun olamaz.", "COURSE_DESCRIPTION_TOO_LONG");

            if(price.Amount<0)
                throw new ArgumentException("Kurs fiyatı negatif olamaz.", "COURSE_PRICE_NEGATIVE");

            var course = new Course(title, description, instructorName, instructorEmail, price, level);
            course.CreatedBy = createdBy;

            course.AddDomainEvent(new CourseCreatedDomainEvent(course.ObjectId,course.Title,course.InstructorEmail));

            return course;
        }


        // kurs bilgilerini güncelleme metodu
        public void UpdateDetails(string title, string description, Money money, CourseLevel level,
            int? updatedBy = null)
        {

            if(string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Kurs başlığı boş olamaz.", "COURSE_TITLE_REQUIRED");


            Title = title;
            Description = description;
            Price = money;
            Level = level;

            SetUpdatedInfo(updatedBy);
            AddDomainEvent(new CourseUpdatedDomainEvent(ObjectId,Title));
        }

        //kursu yayınlama metodu
        public void Publish(int? publishedBy = null)
        {
            if (_lessons.Any())
                throw new BusinessRuleViolationException("COURSE_PUBLISH", "En az bir ders olmadan kurs yayınlanamaz");
            if (_categories.Any())
            {
                throw new BusinessRuleViolationException("COURSE_PUBLISH", "En az bir kategori olmadan kurs yayınlanamaz");
            }

            Status = CourseStatus.Published;
            PublishedAt = DateTime.UtcNow;
            SetUpdatedInfo(publishedBy);

            AddDomainEvent(new CoursePublishedDomainEvent(ObjectId, Title, InstructorEmail));
        }

        public void UnPublish(string reason, int? unPublishedBy = null)
        {
            //if(Status != CourseStatus.Publis)
            Status = CourseStatus.Unpublished;
            SetUpdatedInfo(unPublishedBy);
            AddDomainEvent(new CourseUnpublishedDomainEvent(ObjectId, Title, reason));
        }


        //ders ekleme metodu
        public void AddLesson(string title, string description, Duration duration, ContentType contentType, int order,
            int? addedBy = null)
        {
            // Order kontrolü
            if (_lessons.Any(l => l.Order == order))
                throw new BusinessRuleViolationException("LESSON_ORDER", $"Bu sıra numarası ({order}) zaten kullanımda");

            var lesson = Lesson.Create(ObjectId, title, description, duration, contentType, order, addedBy);
            _lessons.Add(lesson);

            RecalculateTotalDuration();
            SetUpdatedInfo(addedBy);
            AddDomainEvent(new LessonAddedDomainEvent(ObjectId, lesson.ObjectId, lesson.Title));
        }

        //ders silme metodu
        public void RemoveLesson(int LessonId, int? removedBy = null)
        {
            var lesson = _lessons.FirstOrDefault(l => l.ObjectId == LessonId);
            if (lesson == null)
                throw new EntityNotFoundException("Lesson", LessonId);

            //Toplam süreyi güncelle
            RecalculateTotalDuration();

            //Sıra numaralarını yeniden düzenle
            ReorderLessons();

            SetUpdatedInfo(removedBy);
            AddDomainEvent(new LessonRemovedDomainEvent(ObjectId,LessonId,lesson.Title));
        }

        //Kategori ekleme
        public void AddCategory(Category category, int? adddedBy = null)
        {
            if(_categories.Any(cc=>cc.CategoryId == category.ObjectId))
                return;
            var courseCategory = CourseCategory.Create(ObjectId, category.ObjectId);
            _categories.Add(courseCategory);
            SetUpdatedInfo(adddedBy);
            AddDomainEvent(new CategoryAddedToCourseDomainEvent(ObjectId, category.ObjectId, category.Name));
        }

        //Kategori silme
        public void RemoveFromCategory(int categoryId, int? removedBy = null)
        {
            var courseCategory = _categories.FirstOrDefault(cc => cc.CategoryId == categoryId);
            if (courseCategory != null)
            {
                _categories.Remove(courseCategory);
                SetUpdatedInfo(removedBy);
                AddDomainEvent(new CategoryRemovedFromCourseDomainEvent(ObjectId, categoryId));
            }
        }



        // Thumbnail URL'sini ayarla
        public void SetThumbnailUrl(string url, int? updatedBy = null)
        {
            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentException("Thumbnail URL boş olamaz.", "COURSE_THUMBNAIL_REQUIRED");
            ThumbnailUrl = url;
            SetUpdatedInfo(updatedBy);

            //add domain event kullanmadık çünkü herhangi bir yeri etkilemiyor başka bir yerde işlenmiyor
        }

        // Trailer video URL'sini ayarla
        public void SetTrailerVideoUrl(string url, int? updatedBy = null)
        {
            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentException("Trailer video URL boş olamaz.", "COURSE_TRAILER_REQUIRED");
            TrailerVideoUrl = url;
            SetUpdatedInfo(updatedBy);
        }

        // Maksimum öğrenci sayısını ayarla
        public void SetMaxStudents(int maxStudents, int? updatedBy = null)
        {
            if (maxStudents <= 0)
                throw new ArgumentException("Maksimum öğrenci sayısı 0'dan büyük olmalıdır.", "COURSE_MAX_STUDENTS_INVALID");
            if (maxStudents < CurrentStudentCount)
                throw new BusinessRuleViolationException("MAX_STUDENTS", "Maksimum öğrenci sayısı mevcut öğrenci sayısından az olamaz");
            MaxStudents = maxStudents;
            SetUpdatedInfo(updatedBy);
        }

        //Öğrenci sayısını artır
        public void IncrementStudentCount(int incrementBy = 1, int? updatedBy = null)
        {
            if (incrementBy <= 0)
                throw new ArgumentException("Artış miktarı 0'dan büyük olmalıdır.", "COURSE_INCREMENT_INVALID");
            if (CurrentStudentCount + incrementBy > MaxStudents)
                throw new BusinessRuleViolationException("COURSE_FULL", "Kurs kapasitesi dolmuştur");
            CurrentStudentCount += incrementBy;

            //süreç içiinde değişecek olan bir şey olmadığı için domain event eklemedik.Öğrenci sayısı artıp azaldığında başka bir bounded context veya event listener’ın bilmesi gerekiyorsa, AddDomainEvent ile event fırlatmak tercih edilebilir.
        }

        // Öğrenci sayısını azalt
        public void DecrementStudentCount(int decrementBy = 1, int? updatedBy = null)
        {
            if (decrementBy <= 0)
                throw new ArgumentException("Azaltma miktarı 0'dan büyük olmalıdır.", "COURSE_DECREMENT_INVALID");
            if (CurrentStudentCount - decrementBy < 0)
                throw new BusinessRuleViolationException("COURSE_EMPTY", "Kurs öğrenci sayısı 0'dan az olamaz");
            CurrentStudentCount -= decrementBy;
        }

        public bool IsFull()
        {
            return CurrentStudentCount >= MaxStudents;
        }

        public bool IsFree => Price.Amount == 0;

        public bool IsAvailable=>Status == CourseStatus.Published && !IsFull();

        // Private helper methods
        private void RecalculateTotalDuration()
        {
            var totalMinutes = _lessons.Sum(l => l.Duration.TotalMinutes);
            TotalDuration = Duration.FromMinutes(totalMinutes);
        }

        private void ReorderLessons()
        {
            var orderedLessons = _lessons.OrderBy(l => l.Order).ToList();
            for (int i = 0; i < orderedLessons.Count; i++)
            {
                orderedLessons[i].UpdateOrder(i + 1);
            }
        }
    }
}
