using ELearningIskoop.BuildingBlocks.Application.CQRS;
using ELearningIskoop.BuildingBlocks.Domain;
using ELearningIskoop.Courses.Domain.Entities;
using ELearningIskoop.Courses.Domain.Enums;
using ELearningIskoop.Courses.Domain.Repositories;
using ELearningIskoop.Courses.Domain.Services;
using ELearningIskoop.Shared.Domain.Enums;
using ELearningIskoop.Shared.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace ELearningIskoop.Courses.Application.Commands.CreateCourse
{
    public class CreateCourseCommandHandler : ICommandHandler<CreateCourseCommand, CreateCourseResponse>
    {
        private readonly ICourseRepository _courseRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly CourseDomainService _courseDomainService;
        private readonly IUnitOfWork _unitOfWork;

        public CreateCourseCommandHandler(ICourseRepository courseRepository, ICategoryRepository categoryRepository,
            CourseDomainService courseDomainService, IUnitOfWork unitOfWork)
        {
            _courseRepository = courseRepository;
            _categoryRepository = categoryRepository;
            _courseDomainService = courseDomainService;
            _unitOfWork = unitOfWork;
        }

        public async Task<CreateCourseResponse> Handle(CreateCourseCommand request, CancellationToken cancellationToken)
        {
            // Business rule kontrolleri
            await ValidateBusinessRulesAsync(request, cancellationToken);

            //Value Objectleri oluştur
            var instructorName = PersonName.Create(request.InstructorFirstName, request.InstructorLastName);
            var instructorEmail = Email.Create(request.InstructorEmail);
            var price = Money.Create(request.Price, request.Currency);

            //Kurs oluştur
            var course = Course.Create(
                title: request.Title,
                description:request.Description,
                instructorName: instructorName,
                instructorEmail: instructorEmail,
                price:price,
                level:request.Level,
                createdBy:request.RequestedBy.Value);

            //opsiyonel alanlar
            if(request.MaxStudents > 0)
            {
                course.SetMaxStudents(request.MaxStudents);
            }
            if (!string.IsNullOrEmpty(request.ThumbnailUrl))
            {
                course.SetThumbnailUrl(request.ThumbnailUrl, request.RequestedBy);
            }

            if(!string.IsNullOrEmpty(request.TrailerVideoUrl))
            {
                course.SetTrailerVideoUrl(request.TrailerVideoUrl, request.RequestedBy);
            }

            //Kategorileri ekle
            await AddCategoriesToCourseAsync(course, request.CategoryIds, request.RequestedBy.Value, cancellationToken);

            //Reporsitory'e ekle
            await _courseRepository.AddAsync(course, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            // Response oluştur
            return new CreateCourseResponse
            {
                CourseId = course.ObjectId,
                Title = course.Title,
                Status = course.Status.GetDescription(),
                CreatedAt = course.CreatedAt
            };
        }

        private async Task ValidateBusinessRulesAsync(CreateCourseCommand request, CancellationToken cancellationToken)
        {
            // Eğitmen kurs oluşturma limiti kontrolü
            var canCreateMore = await _courseDomainService.CanInstructorCreateMoreCourseAsync(request.InstructorEmail);
            if (!canCreateMore)
            {
                throw new BusinessRuleViolationException("INSTRUCTOR_COURSE_LIMIT",
                    "Eğitmen maksimum kurs oluşturma limitine ulaşmıştır");
            }

            //Aynı eğitmen ve başlıkta kurs var mı
            var isUnique = await _courseDomainService.IsCourseUniqueAsync(request.Title, request.InstructorEmail);
            if (!isUnique)
            {
                throw new BusinessRuleViolationException("DUPLICATE_COURSE",
                    "Bu eğitmenin aynı başlıkla bir kursu zaten bulunmaktadır");
            }

            //Kategoriler mevcut mu
            foreach (var categoryId in request.CategoryIds)
            {
                var category = await _categoryRepository.GetByIdAsync(categoryId, cancellationToken);
                if (category == null)
                {
                    throw new EntityNotFoundException("Category", categoryId);
                }

                if (!category.IsActive)
                {
                    throw new BusinessRuleViolationException("INACTIVE_CATEGORY",
                        $"Kategori {categoryId} aktif değil");
                }
            }
        }

        private async Task AddCategoriesToCourseAsync(Course course, List<int> categoryIds, int requestedBy, CancellationToken cancellationToken)
        {
            foreach (var categoryId in categoryIds)
            {
                var category = await _categoryRepository.GetByIdAsync(categoryId, cancellationToken);
                if (category != null)
                {
                    course.AddCategory(category, requestedBy);
                }
            }
        }
    }
}
