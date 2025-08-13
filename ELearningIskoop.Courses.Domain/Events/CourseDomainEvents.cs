using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ELearningIskoop.BuildingBlocks.Domain;
using ELearningIskoop.Shared.Domain.ValueObjects;

namespace ELearningIskoop.Courses.Domain.Events
{
    //Yeni kurs oluşturulduğunda tetiklenecek olay
    public record CourseCreatedDomainEvent(int CourseId, string Title, Email InstructorEMail) : DomainEvent;

    // Kurs güncellendiğinde tetiklenen domain event
    public record CourseUpdatedDomainEvent(int CourseId, string Title) : DomainEvent;

    // Kurs yayınlandığında tetiklenen domain event
    public record CoursePublishedDomainEvent(int CourseId, string Title, Email InstructorEMail) : DomainEvent;

    // Kurs yayından kaldırıldığında tetiklenen domain event
    public record CourseUnpublishedDomainEvent(int CourseId, string Title,string Reason) : DomainEvent;

    // Kursa ders eklendiğinde tetiklenen domain event
    public record LessonAddedDomainEvent(int CourseId, int LessonId, string LessonTitle) : DomainEvent;

    // Kurstan ders çıkarıldığında tetiklenen domain event
    public record LessonRemovedDomainEvent(int CourseId, int LessonId, string LessonTitle) : DomainEvent;

    // Kursa kategori eklendiğinde tetiklenen domain event
    public record CategoryAddedToCourseDomainEvent(int CourseId,int CategoryId, string CategoryName) : DomainEvent;

    // Kurstan kategori çıkarıldığında tetiklenen domain event
    public record CategoryRemovedFromCourseDomainEvent(int CourseId, int CategoryId) : DomainEvent;
}
