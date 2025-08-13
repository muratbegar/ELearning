using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace ELearningIskoop.Courses.Application.Commands.AddLesson
{
    public class AddLessonCommandValidator : AbstractValidator<AddLessonCommand>
    {
        public AddLessonCommandValidator()
        {
            RuleFor(x => x.CourseId).NotEmpty().WithMessage("Kurs ID zorunludur");
            RuleFor(x => x.Title).NotEmpty().WithMessage("Ders başlığı zorunludur").MaximumLength(200).WithMessage("Ders başlığı 200 karakterden fazla olamaz.");
            RuleFor(x => x.Description).NotEmpty().WithMessage("Ders açıklaması zorunludur").MaximumLength(1000)
                .WithMessage("Ders açıklaması 1000 karakterden fazla olamaz.");
            RuleFor(x=>x.DurationMinutes).GreaterThan(0).WithMessage("Ders süresi 0'dan büyük olmalıdır").LessThanOrEqualTo(10080).WithMessage("Ders " +
                "süresi 10080 dakikadan fazla olamaz (7 gün).");
            RuleFor(x => x.ContentType).IsInEnum().WithMessage("Geçerli bir içerik tipi seçiniz");
            RuleFor(x => x.Order).GreaterThan(0).WithMessage("Ders sırası 0 dan büyük olmalıdır");
            RuleFor(x => x.ContentUrl)
                .Must(url => string.IsNullOrEmpty(url) || Uri.IsWellFormedUriString(url, UriKind.Absolute))
                .WithMessage("Geçerli bir içerik URL'si giriniz (eğer içerik URL'si boş değilse)");
        }
    }
}
