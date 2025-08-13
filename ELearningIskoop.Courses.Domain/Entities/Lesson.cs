using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ELearningIskoop.BuildingBlocks.Domain;
using ELearningIskoop.Shared.Domain.Enums;
using ELearningIskoop.Shared.Domain.ValueObjects;

namespace ELearningIskoop.Courses.Domain.Entities
{
    public class Lesson : BaseEntity, IAuditableEntity
    {
        protected Lesson()
        {
        }

        private Lesson(int courseId, string title, string description, Duration duration, ContentType contentType,
            int order)
        {
            CourseId = courseId;
            Title = title;
            Description = description;
            Duration = duration;
            ContentType = contentType;
            Order = order;
            IsPublished = false;
            CreatedAt = DateTime.UtcNow;
        }


        public int CourseId { get; private set; }
        public string Title { get; private set; } = string.Empty;
        public string Description { get; private set; } = string.Empty;
        public Duration Duration { get; private set; } = null!;
        public ContentType ContentType { get; private set; }
        public int Order { get; private set; }
        public bool IsPublished { get; private set; }
        public string? VideoUrl { get; private set; }
        public string? DocumentUrl { get; private set; }
        public string? AudioUrl { get; private set; }

        public string? InteractiveContent { get; private set; }

        public bool IsFree { get; private set; } // Ücretsiz ders mi?

        public Course Course { get; private set; } = null!;


        // ders oluşturma
        public static Lesson Create(int courseId, string title, string description, Duration duration,
            ContentType contentType, int order, int? createdBy = null)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Ders başlığı boş olamaz.", "LESSON_TITLE_REQUIRED");
            if (title.Length > 200)
                throw new DomainException("Ders başlığı 200 karakterden uzun olamaz", "LESSON_TITLE_TOO_LONG");

            if (order <= 0)
                throw new DomainException("Ders sırası 0'dan büyük olmalıdır", "INVALID_LESSON_ORDER");

            var lesson = new Lesson(courseId, title, description, duration, contentType, order);
            lesson.CreatedBy = createdBy;

            return lesson;
        }

        // Ders bilgilerini güncelle
        public void Update(string title, string description, Duration duration, int? updatedBy = null)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Ders başlığı boş olamaz.", "LESSON_TITLE_REQUIRED");
            if (title.Length > 200)
                throw new DomainException("Ders başlığı 200 karakterden uzun olamaz", "LESSON_TITLE_TOO_LONG");

            Title = title;
            Description = description;
            Duration = duration;

            SetUpdatedInfo(updatedBy);
        }

        // Ders sırasını güncelle
        public void UpdateOrder(int newOrder)
        {
            if (newOrder <= 0)
                throw new DomainException("Ders sırası 0'dan büyük olmalıdır.", "LESSON_ORDER_INVALID");

            Order = newOrder;
        }

        //video URL'si değiştir
        public void SetVideoUrl(string videoUrl, int? updatedBy = null)
        {
            if (ContentType != ContentType.Video && ContentType != ContentType.LiveStream)
                throw new BusinessRuleViolationException("LESSON_CONTENT", "Bu ders tipi için video URL ayarlanamaz");
            VideoUrl = videoUrl;
            SetUpdatedInfo(updatedBy);
        }

        // Doküman URL'sini ayarla
        public void SetDocumentUrl(string documentUrl, int? updatedBy = null)
        {
            if (ContentType != ContentType.Document)
                throw new BusinessRuleViolationException("LESSON_CONTENT", "Bu ders tipi için doküman URL ayarlanamaz");
            DocumentUrl = documentUrl;
            SetUpdatedInfo(updatedBy);
        }

        // Ses URL'sini ayarla
        public void SetAudioUrl(string audioUrl, int? updatedBy = null)
        {
            if (ContentType != ContentType.Audio)
                throw new BusinessRuleViolationException("LESSON_CONTENT", "Bu ders tipi için ses URL ayarlanamaz");
            AudioUrl = audioUrl;
            SetUpdatedInfo(updatedBy);
        }

        // Etkileşimli içerik ayarla
        public void SetInteractiveContent(string content, int? updatedBy = null)
        {
            if (!ContentType.IsInteractive())
                throw new BusinessRuleViolationException("LESSON_CONTENT",
                    "Bu ders tipi için etkileşimli içerik ayarlanamaz");
            InteractiveContent = content;
            SetUpdatedInfo(updatedBy);
        }

        // Ücretsiz ders olarak işaretle
        public void MarkAsFree(int? updatedBy = null)
        {
            IsFree = true;
            SetUpdatedInfo(updatedBy);
        }

        // Ücretli ders olarak işaretle
        public void MarkAsPaid(int? updatedBy = null)
        {
            IsFree = false;
            SetUpdatedInfo(updatedBy);
        }

        // Yayınla
        public void Publish(int? publishedBy = null)
        {
            //içerik kontrolü
            var hasContent = ContentType switch
            {
                ContentType.Video => !string.IsNullOrWhiteSpace(VideoUrl),
                ContentType.Document => !string.IsNullOrWhiteSpace(DocumentUrl),
                ContentType.Audio => !string.IsNullOrWhiteSpace(AudioUrl),
                ContentType.Interactive => !string.IsNullOrWhiteSpace(InteractiveContent),
                _ => true // Diğer türler için içerik zorunlu değil
            };

            if (!hasContent)
                throw new BusinessRuleViolationException("LESSON_PUBLISH", "Bu ders için içerik eksik, yayınlanamaz");

            IsPublished = true;
            SetUpdatedInfo(publishedBy);
        }

        // Yayından kaldır
        public void Unpublish(int? unPublishedBy = null)
        {
            if (!IsPublished)
                throw new DomainException("LESSON_NOT_PUBLISHED", "Bu ders zaten yayınlanmamış");
            IsPublished = false;
            SetUpdatedInfo(unPublishedBy);
        }

        // İçerik URL'sini döner (tipi ne olursa olsun)
        public string GetContentUrl()
        {
            return ContentType switch
            {
                ContentType.Video => VideoUrl ?? string.Empty,
                ContentType.Document => DocumentUrl ?? string.Empty,
                ContentType.Audio => AudioUrl ?? string.Empty,
                ContentType.Interactive => InteractiveContent ?? string.Empty,
                _ => throw new DomainException("LESSON_CONTENT", "Bu ders tipi için içerik URL bulunamadı")
            };
        }

        public bool HasContent => !string.IsNullOrEmpty(GetContentUrl());
    }
}
