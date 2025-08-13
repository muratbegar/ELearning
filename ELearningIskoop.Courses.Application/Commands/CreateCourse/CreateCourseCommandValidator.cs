using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Validators;

namespace ELearningIskoop.Courses.Application.Commands.CreateCourse
{
    // Kurs oluşturma komutu validator'ı
    public class CreateCourseCommandValidator : AbstractValidator<CreateCourseCommand>
    {
        public CreateCourseCommandValidator()
        {
            RuleFor(x => x.Title).NotEmpty().WithMessage("Kurs Başlığı zorunludur").MaximumLength(200)
                .WithMessage("Kurs başlığı 2000 karakterden uzun olamaz");

            RuleFor(x => x.Description).NotEmpty().WithMessage("Kurs Açıklaması zorunludur").MaximumLength(2000).WithMessage("Kurs açıklaması 2000 karakterden uzun olamaz");

            RuleFor(x => x.InstructorFirstName).NotEmpty().WithMessage("Eğitmen Adı zorunludur").MaximumLength(100)
                .WithMessage("Eğitmen adı 100 karakterden uzun olamaz").MinimumLength(2).WithMessage("2 karakterden kısa olamaz");

            RuleFor(x => x.InstructorLastName).NotEmpty().WithMessage("Eğitmen Soyadı zorunludur").MaximumLength(100)
                .WithMessage("Eğitmen soyadı 100 karakterden uzun olamaz").MinimumLength(2)
                .WithMessage("2 karakterden kısa olamaz");

            RuleFor(x => x.InstructorEmail).NotEmpty().WithMessage("Eğitmen e-mail adresi zorunludur").EmailAddress()
                .WithMessage("Geçerli bir e-mail formatı girin");

            RuleFor(x => x.Price).GreaterThan(0).WithMessage("Kurs fiyatı 0'dan büyük olmalıdır");

            RuleFor(x => x.Currency).NotEmpty().WithMessage("Para birimi zorunludur").Must(BeValidCurrency)
                .WithMessage("Geçersiz para birimi");

            RuleFor(x => x.Level).IsInEnum().WithMessage("Kurs seviyesi geçersiz");
            RuleFor(x => x.MaxStudents).GreaterThan(0).WithMessage("Maksimum öğrenci sayısı 0'dan büyük olmalıdır").LessThanOrEqualTo(10000).WithMessage("Maksimum öğrenci sayısı 10000 i geçemez");

            RuleFor(x => x.CategoryIds).NotEmpty().WithMessage("En az bir kategori seçilmelidir").Must(x => x.Count < 5)
                .WithMessage("En fazla 5 kategori seçilebilir");
            RuleFor(x => x.ThumbnailUrl).Must(BeValidUrl).WithMessage("Geçerli bir thumbnail URL'si girin");

            RuleFor(x => x.TrailerVideoUrl)
                .Must(BeValidUrl)
                .When(x => !string.IsNullOrEmpty(x.TrailerVideoUrl))
                .WithMessage("Geçerli bir trailer video URL'si giriniz");
        }

        public static bool BeValidCurrency(string currency)
        {
            // Geçerli para birimleri
            var validCurrencies = new List<string> { "TRY", "USD", "EUR", "GBP" };
            return validCurrencies.Contains(currency.ToUpperInvariant());
        }

        private static bool BeValidUrl(string? url)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out _);
        }
    }
}
