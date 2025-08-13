using ELearningIskoop.Courses.Domain.Repositories;
using ELearningIskoop.Courses.Domain.Services;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ELearningIskoop.Courses.Application.Commands.CreateCourse;

namespace ELearningIskoop.Courses.Application.Behaviors
{
    // Kurs modülü için özel validation behavior'u
    // Database tabanlı validasyonlar için
    public class CourseValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull
    {
        private readonly CourseDomainService _courseDomainService;
        private readonly ICategoryRepository _categoryRepository;

        public CourseValidationBehavior(
            CourseDomainService courseDomainService,
            ICategoryRepository categoryRepository)
        {
            _courseDomainService = courseDomainService;
            _categoryRepository = categoryRepository;
        }
        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            // Sadece CreateCourseCommand için ek validasyon yap
            if (request is CreateCourseCommand createCommand)
            {
                await ValidateCreateCourseCommandAsync(createCommand, cancellationToken);
            }

            return await next();
        }

        private async Task ValidateCreateCourseCommandAsync(CreateCourseCommand command,
            CancellationToken cancellationToken)
        {
            var errors = new List<FluentValidation.Results.ValidationFailure>();

            //eğitmen kurs limitini kontrol et
            var canCreateMore = await _courseDomainService.CanInstructorCreateMoreCourseAsync(command.InstructorEmail);
            if (!canCreateMore)
            {
                errors.Add(new FluentValidation.Results.ValidationFailure(nameof(command.InstructorEmail),
                    "Eğitmen maksimum kurs sayısına ulaştı."));
            }

            //kurs tekil kontrolü
            var isUnique = await _courseDomainService.IsCourseUniqueAsync(command.Title, command.InstructorEmail);
            if (!isUnique)
            {
                errors.Add(new FluentValidation.Results.ValidationFailure(nameof(command.Title),
                    "Bu eğitmenin zaten bu başlıkta bir kursu var."));
            }


            // Kategori varlık kontrolü
            foreach (var categoryId in command.CategoryIds)
            {
                var category = await _categoryRepository.GetByIdAsync(categoryId, cancellationToken);
                if(category == null)
                {
                    errors.Add(new FluentValidation.Results.ValidationFailure(nameof(command.CategoryIds),
                        $"Kategori ID {categoryId} bulunamadı."));
                }
                else if (!category.IsActive)
                {
                    errors.Add(new FluentValidation.Results.ValidationFailure(nameof(command.CategoryIds),
                        $"Kategori ID {categoryId} aktif değil."));
                }
            }

            if (errors.Any())
            {
                throw new FluentValidation.ValidationException(errors);
            }
        }
    }
}
