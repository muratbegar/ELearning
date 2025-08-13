using ELearningIskoop.BuildingBlocks.Domain;
using ELearningIskoop.BuildingBlocks.Domain.Repositories;
using ELearningIskoop.Courses.Domain.Entities;
using ELearningIskoop.Courses.Domain.Enums;
using ELearningIskoop.Courses.Domain.Repositories;
using ELearningIskoop.Courses.Infrastructure.Persistence;
using ELearningIskoop.Shared.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ELearningIskoop.Courses.Infrastructure.Repositories
{
    public class CourseRepository : ICourseRepository
    {
        private readonly CoursesDbContext _context;

        public CourseRepository(CoursesDbContext context)
        {
            _context = context;
        }


        public async Task<Course?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Courses.Include(c => c.Lessons.OrderBy(x => x.Order)).Include(c => c.Categories)
                    .ThenInclude(cc => cc.Category).FirstOrDefaultAsync(c => c.ObjectId == id, cancellationToken)
                ;


            //sadece akttif kurslar için performanslı sorgu
            //return await _context.Courses
            //   .AsSplitQuery() // Birden fazla SQL sorgusu oluşturur
            //   .Include(c => c.Lessons.Where(l => !l.IsDeleted).OrderBy(l => l.Order))
            //   .Include(c => c.Categories.Where(cc => cc.Category.IsActive))
            //   .ThenInclude(cc => cc.Category)
            //   .FirstOrDefaultAsync(c => c.ObjectId == id, cancellationToken);

            //Include ve ThenInclude Entity Framework'ün lazy loading yerine ilişkili verileri önceden yükleme mekanizmalarıdır.
            //Include, ana entity ile ilişkili olan entity'leri yüklerken, ThenInclude ise Include ile yüklenen entity'nin ilişkili olan entity'lerini yükler.
            //.Include(c => c.Categories)        // Course -> CourseCategories
            //.ThenInclude(cc => cc.Category)    // CourseCategories -> Category

            // SELECT c.*, l.*, cc.*, cat.*
            //     FROM Courses c
            // LEFT JOIN Lessons l ON c.Id = l.CourseId
            // LEFT JOIN CourseCategories cc ON c.Id = cc.CourseId
            // LEFT JOIN Categories cat ON cc.CategoryId = cat.Id
            // WHERE c.ObjectId = @id
            // ORDER BY l.Order
        }

        public async Task<IEnumerable<Course>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Courses.Include(c => c.Lessons).Include(c => c.Categories)
                .ThenInclude(cc => cc.Category).OrderByDescending(c => c.CreatedAt).ToListAsync(cancellationToken);

            //Sorgunun Yaptığı İşlemler:
            //
            //Tüm Course'ları getirir (GetAllAsync - filtreleme yok)
            //Her Course'un Lessons'larını yükler(.Include(c => c.Lessons))
            //Her Course'un Categories'lerini yükler(.Include(c => c.Categories))
            //Her CourseCategory'nin Category detayını yükler (.ThenInclude(cc => cc.Category))
            //Sonuçları oluşturulma tarihine göre yeniden eskiye sıralar(.OrderByDescending(c => c.CreatedAt))
            //Listeyi döndürür(.ToListAsync())
        }

        public async Task<IEnumerable<Course>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            return await _context.Courses.Include(c => c.Lessons).Include(c => c.Categories)
                .ThenInclude(cc => cc.Category).OrderByDescending(c => c.CreatedAt).Skip((pageNumber - 1) * pageSize)
                .Take(pageSize).ToListAsync(cancellationToken);


            //Kursları sayfa sayfa getirir(pagination)
            //Include: Lessons + Categories + Category detayları
            //Sıralama: CreatedAt'e göre yeniden eskiye
        }

        public async Task<IEnumerable<Course>> FindAsync(Expression<Func<Course, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await _context.Courses.Include(c => c.Lessons).Include(c => c.Categories)
                .ThenInclude(cc => cc.Category).Where(predicate).ToListAsync(cancellationToken);

            // Lambda expression ile filtreleme yapar
        }

        public async Task<Course> GetSingleAsync(Expression<Func<Course, bool>> predicate, CancellationToken cancellationToken = default)
        {
            var course = await _context.Courses.Include(c => c.Lessons.OrderBy(l => l.Order)).Include(c => c.Categories)
                .ThenInclude(cc => cc.Category).FirstOrDefaultAsync(predicate, cancellationToken);
            if (course == null)
                throw new EntityNotFoundException("Course", "predicate");
            return course;

            //Tek kurs getirir, bulamazsa exception fırlatır
            //Lessons'ları Order'a göre sıralar
        }

        public async Task<bool> ExistsAsync(Expression<Func<Course, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await _context.Courses.AnyAsync(predicate, cancellationToken);
            //Belirtilen koşula uyan kurs var mı kontrol eder
        }

        public async Task<int> CountAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Courses.CountAsync(cancellationToken);
            //Toplam kurs sayısı 
        }

        public async Task<int> CountAsync(Expression<Func<Course, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await _context.Courses.CountAsync(predicate, cancellationToken);
            // Koşula uyan kurs sayısı
        }

        public async Task AddAsync(Course entity, CancellationToken cancellationToken = default)
        {
            _context.Courses.Add(entity);
            
        }

        public async Task AddRangeAsync(IEnumerable<Course> entities, CancellationToken cancellationToken = default)
        {
            _context.Courses.AddRange(entities);
        }

        public async Task UpdateAsync(Course entity, CancellationToken cancellationToken = default)
        {
            _context.Courses.Update(entity);
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var course = await GetByIdAsync(id, cancellationToken);
            if (course != null)
            {
                _context.Courses.Remove(course);
            }
        }

        public async Task DeleteAsync(Course entity, CancellationToken cancellationToken = default)
        {
            _context.Courses.Remove(entity);
        }

        public async Task DeleteRangeAsync(IEnumerable<Course> entities, CancellationToken cancellationToken = default)
        {
            _context.Courses.RemoveRange(entities);
        }

        public async Task<IEnumerable<Course>> GetActiveAsync(CancellationToken cancellationToken = default)
        {
            // Soft delete query filter otomatik uygulanıyor
            return await GetAllAsync(cancellationToken);
        }

        public async Task<IEnumerable<Course>> GetDeletedAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Courses
                .IgnoreQueryFilters() // Soft delete filter'ını by-pass et
                .Where(c => c.IsDeleted)
                .Include(c => c.Lessons)
                .Include(c => c.Categories)
                .ThenInclude(cc => cc.Category)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Course>> GetByInstructorEmailAsync(string instructorEmail, CancellationToken cancellationToken = default)
        {
            return await _context.Courses
                .Include(c => c.Lessons)
                .Include(c => c.Categories)
                .ThenInclude(cc => cc.Category)
                .Where(c => c.InstructorEmail.Value == instructorEmail)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync(cancellationToken);

            //Belirli eğitmenin kurslarını getirir
            //Email'e göre filtreleme
        }

        public async Task<IEnumerable<Course>> GetByLevelAsync(CourseLevel level, CancellationToken cancellationToken = default)
        {
            return await _context.Courses
                .Include(c => c.Lessons)
                .Include(c => c.Categories)
                .ThenInclude(cc => cc.Category)
                .Where(c => c.Level == level)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync(cancellationToken);

            //Belirli seviyedeki kursları getirir (Beginner, Intermediate, Advanced)
        }

        public async Task<IEnumerable<Course>> GetByCategoryAsync(int categoryId, CancellationToken cancellationToken = default)
        {
            return await _context.Courses
                .Include(c => c.Lessons)
                .Include(c => c.Categories)
                .ThenInclude(cc => cc.Category)
                .Where(c => c.Categories.Any(cc => cc.CategoryId == categoryId))
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync(cancellationToken);

            //Belirli kategorideki kursları getirir
            //Many - to - many ilişki kullanır
        }

        public async Task<IEnumerable<Course>> GetPublishedCoursesAsync(int pageNumber = 1, int pageSize = 20, CancellationToken cancellationToken = default)
        {
            return await _context.Courses
                .Include(c => c.Lessons)
                .Include(c => c.Categories)
                .ThenInclude(cc => cc.Category)
                .Where(c => c.Status == Domain.Enums.CourseStatus.Published)
                .OrderByDescending(c => c.PublishedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            //Sadece yayınlanmış kursları getirir
            //Sıralama: PublishedAt'e göre
        }

        public async Task<IEnumerable<Course>> GetFreeCoursesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Courses
                .Include(c => c.Lessons)
                .Include(c => c.Categories)
                .ThenInclude(cc => cc.Category)
                .Where(c => c.Price.Amount == 0 && c.Status == Domain.Enums.CourseStatus.Published)
                .OrderByDescending(c => c.PublishedAt)
                .ToListAsync(cancellationToken);

            //Ücretsiz ve yayınlanmış kursları getirir
            //Price.Amount == 0 koşulu
        }

        public async Task<IEnumerable<Course>> SearchAsync(string searchTerm, CourseLevel? level = null, int? categoryId = null, bool? isFree = null,
            int pageNumber = 1, int pageSize = 20, CancellationToken cancellationToken = default)
        {
            var query = _context.Courses
                .Include(c => c.Lessons)
                .Include(c => c.Categories)
                .ThenInclude(cc => cc.Category)
                .Where(c => c.Status == Domain.Enums.CourseStatus.Published);

            // Text search
            if (!string.IsNullOrEmpty(searchTerm))
            {
                var searchLower = searchTerm.ToLower();
                query = query.Where(c =>
                    c.Title.ToLower().Contains(searchLower) ||
                    c.Description.ToLower().Contains(searchLower) ||
                    c.InstructorName.FirstName.ToLower().Contains(searchLower) ||
                    c.InstructorName.LastName.ToLower().Contains(searchLower));
            }

            // Level filter
            if (level.HasValue)
            {
                query = query.Where(c => c.Level == level.Value);
            }

            // Category filter
            if (categoryId.HasValue)
            {
                query = query.Where(c => c.Categories.Any(cc => cc.CategoryId == categoryId.Value));
            }

            // Free filter
            if (isFree.HasValue)
            {
                if (isFree.Value)
                    query = query.Where(c => c.Price.Amount == 0);
                else
                    query = query.Where(c => c.Price.Amount > 0);
            }

            return await query
                .OrderByDescending(c => c.CurrentStudentCount) // Popularity sorting
                .ThenByDescending(c => c.PublishedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);


           //Gelişmiş arama fonksiyonu:
           //
           //Text arama: Title, Description, Instructor name'de arar
           //Level filtreleme
           //Category filtreleme
           //Ücretsiz / ücretli filtreleme
           //
           //
           //Sıralama: Popülarite(CurrentStudentCount) → PublishedAt
        }

        public async Task<IEnumerable<Course>> GetPopularCoursesAsync(int count = 10, CancellationToken cancellationToken = default)
        {
            return await _context.Courses
                .Include(c => c.Lessons)
                .Include(c => c.Categories)
                .ThenInclude(cc => cc.Category)
                .Where(c => c.Status == Domain.Enums.CourseStatus.Published)
                .OrderByDescending(c => c.CurrentStudentCount)
                .ThenByDescending(c => c.PublishedAt)
                .Take(count)
                .ToListAsync(cancellationToken);

            //En popüler kursları getirir
            //Sıralama: CurrentStudentCount → PublishedAt
        }

        public async Task<CourseStatistics> GetStatisticsAsync(int courseId, CancellationToken cancellationToken = default)
        {
            var totalCourses = await _context.Courses.CountAsync(cancellationToken);
            var publishedCourses = await _context.Courses.CountAsync(c => c.Status == Domain.Enums.CourseStatus.Published, cancellationToken);
            var draftCourses = await _context.Courses.CountAsync(c => c.Status == Domain.Enums.CourseStatus.Draft, cancellationToken);
            var totalLessons = await _context.Lessons.CountAsync(cancellationToken);
            var totalStudents = await _context.Courses.SumAsync(c => c.CurrentStudentCount, cancellationToken);
            var totalInstructors = await _context.Courses.Select(c => c.InstructorEmail.Value).Distinct().CountAsync(cancellationToken);

            // Average rating - şimdilik placeholder (rating modülü henüz yok)
            var averageRating = 0m;

            // Total duration
            var totalDurationMinutes = await _context.Courses.SumAsync(c => c.TotalDuration.TotalMinutes, cancellationToken);
            var totalContentDuration = TimeSpan.FromMinutes(totalDurationMinutes);

            return new CourseStatistics(
                totalCourses,
                publishedCourses,
                draftCourses,
                totalLessons,
                totalStudents,
                totalInstructors,
                averageRating,
                totalContentDuration);

            //Genel istatistikleri döndürür:
            //
            //Toplam kurs sayısı
            //    Yayınlanan kurs sayısı
            //Taslak kurs sayısı
            //    Toplam ders sayısı
            //Toplam öğrenci sayısı
            //    Toplam eğitmen sayısı
            //Ortalama rating(placeholder)
            //Toplam içerik süresi
        }

        public Task<Course> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}


//-----------------------------------------------------CLAUDE OPTIMIZSAZYON ÖNERİ---------------------------------------------------------------------------------------
//using Microsoft.EntityFrameworkCore;
//using System.Linq.Expressions;

//public class OptimizedCourseRepository : ICourseRepository
//{
//    private readonly CourseDbContext _context;

//    public OptimizedCourseRepository(CourseDbContext context)
//    {
//        _context = context;
//    }

//    #region Base Queries - Code Duplication'ı Önler

//    /// <summary>
//    /// Temel course query'si - Include'ları tek yerde yönetir
//    /// </summary>
//    private IQueryable<Course> GetBaseQuery(bool includeRelations = true, bool trackChanges = false)
//    {
//        var query = trackChanges ? _context.Courses : _context.Courses.AsNoTracking();

//        if (includeRelations)
//        {
//            query = query
//                .AsSplitQuery() // Performance için
//                .Include(c => c.Lessons.Where(l => !l.IsDeleted).OrderBy(l => l.Order))
//                .Include(c => c.Categories.Where(cc => cc.Category.IsActive))
//                .ThenInclude(cc => cc.Category);
//        }

//        return query;
//    }

//    /// <summary>
//    /// Lightweight query - sadece liste görünümü için
//    /// </summary>
//    private IQueryable<CourseListDto> GetListQuery()
//    {
//        return _context.Courses
//            .AsNoTracking()
//            .Where(c => !c.IsDeleted)
//            .Select(c => new CourseListDto
//            {
//                Id = c.Id,
//                ObjectId = c.ObjectId,
//                Title = c.Title,
//                Description = c.Description.Length > 200
//                    ? c.Description.Substring(0, 200) + "..."
//                    : c.Description,
//                InstructorName = c.InstructorName.FirstName + " " + c.InstructorName.LastName,
//                InstructorEmail = c.InstructorEmail.Value,
//                Price = c.Price.Amount,
//                Currency = c.Price.Currency,
//                Level = c.Level,
//                Status = c.Status,
//                PublishedAt = c.PublishedAt,
//                TotalDurationMinutes = c.TotalDuration.TotalMinutes,
//                CurrentStudentCount = c.CurrentStudentCount,
//                MaxStudents = c.MaxStudents,
//                ThumbnailUrl = c.ThumbnailUrl,
//                LessonCount = c.Lessons.Count(l => !l.IsDeleted),
//                CategoryNames = c.Categories
//                    .Where(cc => cc.Category.IsActive)
//                    .Select(cc => cc.Category.Name)
//                    .ToList(),
//                CreatedAt = c.CreatedAt
//            });
//    }

//    #endregion

//    #region Optimized Read Methods

//    public async Task<Course?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
//    {
//        return await GetBaseQuery()
//            .FirstOrDefaultAsync(c => c.ObjectId == id, cancellationToken);
//    }

//    public async Task<CourseDetailDto?> GetDetailsByIdAsync(int id, CancellationToken cancellationToken = default)
//    {
//        return await _context.Courses
//            .AsNoTracking()
//            .Where(c => c.ObjectId == id && !c.IsDeleted)
//            .Select(c => new CourseDetailDto
//            {
//                Id = c.Id,
//                ObjectId = c.ObjectId,
//                Title = c.Title,
//                Description = c.Description,
//                InstructorName = c.InstructorName.FirstName + " " + c.InstructorName.LastName,
//                InstructorEmail = c.InstructorEmail.Value,
//                Price = c.Price.Amount,
//                Currency = c.Price.Currency,
//                Level = c.Level,
//                Status = c.Status,
//                PublishedAt = c.PublishedAt,
//                TotalDurationMinutes = c.TotalDuration.TotalMinutes,
//                CurrentStudentCount = c.CurrentStudentCount,
//                MaxStudents = c.MaxStudents,
//                ThumbnailUrl = c.ThumbnailUrl,
//                TrailerVideoUrl = c.TrailerVideoUrl,
//                Lessons = c.Lessons
//                    .Where(l => !l.IsDeleted)
//                    .OrderBy(l => l.Order)
//                    .Select(l => new LessonDto
//                    {
//                        Id = l.Id,
//                        Title = l.Title,
//                        Description = l.Description,
//                        DurationMinutes = l.Duration.TotalMinutes,
//                        ContentType = l.ContentType,
//                        Order = l.Order,
//                        IsPublished = l.IsPublished,
//                        IsFree = l.IsFree,
//                        VideoUrl = l.VideoUrl,
//                        DocumentUrl = l.DocumentUrl
//                    }).ToList(),
//                Categories = c.Categories
//                    .Where(cc => cc.Category.IsActive)
//                    .Select(cc => new CategoryDto
//                    {
//                        Id = cc.Category.Id,
//                        Name = cc.Category.Name,
//                        Slug = cc.Category.Slug,
//                        Color = cc.Category.Color,
//                        IconUrl = cc.Category.IconUrl
//                    }).ToList(),
//                CreatedAt = c.CreatedAt,
//                UpdatedAt = c.UpdatedAt
//            })
//            .FirstOrDefaultAsync(cancellationToken);
//    }

//    public async Task<PagedResult<CourseListDto>> GetPagedAsync(
//        int pageNumber,
//        int pageSize,
//        CancellationToken cancellationToken = default)
//    {
//        var query = GetListQuery();

//        var totalCount = await query.CountAsync(cancellationToken);

//        var items = await query
//            .OrderByDescending(c => c.CreatedAt)
//            .Skip((pageNumber - 1) * pageSize)
//            .Take(pageSize)
//            .ToListAsync(cancellationToken);

//        return new PagedResult<CourseListDto>(items, totalCount, pageNumber, pageSize);
//    }

//    public async Task<IEnumerable<Course>> FindAsync(
//        Expression<Func<Course, bool>> predicate,
//        bool includeRelations = true,
//        CancellationToken cancellationToken = default)
//    {
//        return await GetBaseQuery(includeRelations)
//            .Where(predicate)
//            .ToListAsync(cancellationToken);
//    }

//    #endregion

//    #region Specialized Queries with Projection

//    public async Task<PagedResult<CourseListDto>> GetPublishedCoursesAsync(
//        int pageNumber = 1,
//        int pageSize = 20,
//        CancellationToken cancellationToken = default)
//    {
//        var query = GetListQuery()
//            .Where(c => c.Status == CourseStatus.Published);

//        var totalCount = await query.CountAsync(cancellationToken);

//        var items = await query
//            .OrderByDescending(c => c.PublishedAt)
//            .Skip((pageNumber - 1) * pageSize)
//            .Take(pageSize)
//            .ToListAsync(cancellationToken);

//        return new PagedResult<CourseListDto>(items, totalCount, pageNumber, pageSize);
//    }

//    public async Task<IEnumerable<CourseListDto>> GetFreeCoursesAsync(CancellationToken cancellationToken = default)
//    {
//        return await GetListQuery()
//            .Where(c => c.Price == 0 && c.Status == CourseStatus.Published)
//            .OrderByDescending(c => c.PublishedAt)
//            .ToListAsync(cancellationToken);
//    }

//    public async Task<IEnumerable<CourseListDto>> GetPopularCoursesAsync(
//        int count = 10,
//        CancellationToken cancellationToken = default)
//    {
//        return await GetListQuery()
//            .Where(c => c.Status == CourseStatus.Published)
//            .OrderByDescending(c => c.CurrentStudentCount)
//            .ThenByDescending(c => c.PublishedAt)
//            .Take(count)
//            .ToListAsync(cancellationToken);
//    }

//    public async Task<IEnumerable<CourseListDto>> GetByInstructorEmailAsync(
//        string instructorEmail,
//        CancellationToken cancellationToken = default)
//    {
//        return await GetListQuery()
//            .Where(c => c.InstructorEmail == instructorEmail)
//            .OrderByDescending(c => c.CreatedAt)
//            .ToListAsync(cancellationToken);
//    }

//    public async Task<IEnumerable<CourseListDto>> GetByLevelAsync(
//        CourseLevel level,
//        CancellationToken cancellationToken = default)
//    {
//        return await GetListQuery()
//            .Where(c => c.Level == level)
//            .OrderByDescending(c => c.CreatedAt)
//            .ToListAsync(cancellationToken);
//    }

//    public async Task<PagedResult<CourseListDto>> GetByCategoryAsync(
//        int categoryId,
//        int pageNumber = 1,
//        int pageSize = 20,
//        CancellationToken cancellationToken = default)
//    {
//        var query = _context.Courses
//            .AsNoTracking()
//            .Where(c => !c.IsDeleted && c.Categories.Any(cc => cc.CategoryId == categoryId))
//            .Select(c => GetListQuery().First(cl => cl.Id == c.Id));

//        var totalCount = await query.CountAsync(cancellationToken);

//        var items = await query
//            .OrderByDescending(c => c.CreatedAt)
//            .Skip((pageNumber - 1) * pageSize)
//            .Take(pageSize)
//            .ToListAsync(cancellationToken);

//        return new PagedResult<CourseListDto>(items, totalCount, pageNumber, pageSize);
//    }

//    #endregion

//    #region Advanced Search with Caching

//    public async Task<PagedResult<CourseListDto>> SearchAsync(
//        string searchTerm,
//        CourseLevel? level = null,
//        int? categoryId = null,
//        bool? isFree = null,
//        int pageNumber = 1,
//        int pageSize = 20,
//        CancellationToken cancellationToken = default)
//    {
//        var query = GetListQuery()
//            .Where(c => c.Status == CourseStatus.Published);

//        // Full-text search optimization
//        if (!string.IsNullOrEmpty(searchTerm))
//        {
//            var searchLower = searchTerm.ToLower();
//            query = query.Where(c =>
//                EF.Functions.Like(c.Title.ToLower(), $"%{searchLower}%") ||
//                EF.Functions.Like(c.Description.ToLower(), $"%{searchLower}%") ||
//                EF.Functions.Like(c.InstructorName.ToLower(), $"%{searchLower}%"));
//        }

//        if (level.HasValue)
//            query = query.Where(c => c.Level == level.Value);

//        if (categoryId.HasValue)
//            query = query.Where(c => c.CategoryNames.Any()); // Bu kısmı CategoryNames ile optimize etmek gerek

//        if (isFree.HasValue)
//        {
//            if (isFree.Value)
//                query = query.Where(c => c.Price == 0);
//            else
//                query = query.Where(c => c.Price > 0);
//        }

//        var totalCount = await query.CountAsync(cancellationToken);

//        var items = await query
//            .OrderByDescending(c => c.CurrentStudentCount)
//            .ThenByDescending(c => c.PublishedAt)
//            .Skip((pageNumber - 1) * pageSize)
//            .Take(pageSize)
//            .ToListAsync(cancellationToken);

//        return new PagedResult<CourseListDto>(items, totalCount, pageNumber, pageSize);
//    }

//    #endregion

//    #region Count & Exists - No Include Needed

//    public async Task<bool> ExistsAsync(
//        Expression<Func<Course, bool>> predicate,
//        CancellationToken cancellationToken = default)
//    {
//        return await _context.Courses
//            .AsNoTracking()
//            .AnyAsync(predicate, cancellationToken);
//    }

//    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
//    {
//        return await _context.Courses.CountAsync(c => !c.IsDeleted, cancellationToken);
//    }

//    public async Task<int> CountAsync(
//        Expression<Func<Course, bool>> predicate,
//        CancellationToken cancellationToken = default)
//    {
//        return await _context.Courses.CountAsync(predicate, cancellationToken);
//    }

//    #endregion

//    #region Write Operations - Properly Async

//    public void Add(Course entity)
//    {
//        _context.Courses.Add(entity);
//    }

//    public void AddRange(IEnumerable<Course> entities)
//    {
//        _context.Courses.AddRange(entities);
//    }

//    public void Update(Course entity)
//    {
//        _context.Courses.Update(entity);
//    }

//    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
//    {
//        // Soft delete optimization - sadece update
//        await _context.Courses
//            .Where(c => c.ObjectId == id)
//            .ExecuteUpdateAsync(c => c
//                .SetProperty(x => x.IsDeleted, true)
//                .SetProperty(x => x.UpdatedAt, DateTime.UtcNow),
//                cancellationToken);
//    }

//    public void Delete(Course entity)
//    {
//        entity.IsDeleted = true;
//        entity.UpdatedAt = DateTime.UtcNow;
//        _context.Courses.Update(entity);
//    }

//    #endregion

//    #region Statistics - Optimized with Single Query

//    public async Task<CourseStatistics> GetStatisticsAsync(CancellationToken cancellationToken = default)
//    {
//        var stats = await _context.Courses
//            .AsNoTracking()
//            .GroupBy(c => 1) // Trick for aggregate
//            .Select(g => new
//            {
//                TotalCourses = g.Count(),
//                PublishedCourses = g.Count(c => c.Status == CourseStatus.Published),
//                DraftCourses = g.Count(c => c.Status == CourseStatus.Draft),
//                TotalStudents = g.Sum(c => c.CurrentStudentCount),
//                TotalDurationMinutes = g.Sum(c => c.TotalDuration.TotalMinutes)
//            })
//            .FirstAsync(cancellationToken);

//        var totalLessons = await _context.Lessons
//            .AsNoTracking()
//            .CountAsync(l => !l.IsDeleted, cancellationToken);

//        var totalInstructors = await _context.Courses
//            .AsNoTracking()
//            .Select(c => c.InstructorEmail.Value)
//            .Distinct()
//            .CountAsync(cancellationToken);

//        return new CourseStatistics(
//            stats.TotalCourses,
//            stats.PublishedCourses,
//            stats.DraftCourses,
//            totalLessons,
//            stats.TotalStudents,
//            totalInstructors,
//            0m, // Average rating placeholder
//            TimeSpan.FromMinutes(stats.TotalDurationMinutes));
//    }

//    #endregion

//    #region Soft Delete Operations

//    public async Task<IEnumerable<CourseListDto>> GetActiveAsync(CancellationToken cancellationToken = default)
//    {
//        return await GetListQuery() // Zaten IsDeleted filtresi var
//            .ToListAsync(cancellationToken);
//    }

//    public async Task<IEnumerable<CourseListDto>> GetDeletedAsync(CancellationToken cancellationToken = default)
//    {
//        return await _context.Courses
//            .AsNoTracking()
//            .IgnoreQueryFilters()
//            .Where(c => c.IsDeleted)
//            .Select(c => new CourseListDto
//            {
//                Id = c.Id,
//                ObjectId = c.ObjectId,
//                Title = c.Title,
//                Status = c.Status,
//                CreatedAt = c.CreatedAt,
//                UpdatedAt = c.UpdatedAt
//                // Minimal data for deleted courses
//            })
//            .ToListAsync(cancellationToken);
//    }

//    #endregion
//}

//#region DTOs for Performance

//public class CourseListDto
//{
//    public Guid Id { get; set; }
//    public int ObjectId { get; set; }
//    public string Title { get; set; }
//    public string Description { get; set; }
//    public string InstructorName { get; set; }
//    public string InstructorEmail { get; set; }
//    public decimal Price { get; set; }
//    public string Currency { get; set; }
//    public CourseLevel Level { get; set; }
//    public CourseStatus Status { get; set; }
//    public DateTime? PublishedAt { get; set; }
//    public double TotalDurationMinutes { get; set; }
//    public int CurrentStudentCount { get; set; }
//    public int MaxStudents { get; set; }
//    public string? ThumbnailUrl { get; set; }
//    public int LessonCount { get; set; }
//    public List<string> CategoryNames { get; set; } = new();
//    public DateTime CreatedAt { get; set; }
//}

//public class CourseDetailDto : CourseListDto
//{
//    public string? TrailerVideoUrl { get; set; }
//    public List<LessonDto> Lessons { get; set; } = new();
//    public List<CategoryDto> Categories { get; set; } = new();
//    public DateTime? UpdatedAt { get; set; }
//}

//public class LessonDto
//{
//    public Guid Id { get; set; }
//    public string Title { get; set; }
//    public string Description { get; set; }
//    public double DurationMinutes { get; set; }
//    public ContentType ContentType { get; set; }
//    public int Order { get; set; }
//    public bool IsPublished { get; set; }
//    public bool IsFree { get; set; }
//    public string? VideoUrl { get; set; }
//    public string? DocumentUrl { get; set; }
//}

//public class CategoryDto
//{
//    public Guid Id { get; set; }
//    public string Name { get; set; }
//    public string Slug { get; set; }
//    public string? Color { get; set; }
//    public string? IconUrl { get; set; }
//}

//public class PagedResult<T>
//{
//    public IEnumerable<T> Items { get; set; }
//    public int TotalCount { get; set; }
//    public int PageNumber { get; set; }
//    public int PageSize { get; set; }
//    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
//    public bool HasNextPage => PageNumber < TotalPages;
//    public bool HasPrevPage => PageNumber > 1;

//    public PagedResult(IEnumerable<T> items, int totalCount, int pageNumber, int pageSize)
//    {
//        Items = items;
//        TotalCount = totalCount;
//        PageNumber = pageNumber;
//        PageSize = pageSize;
//    }
//}

//#endregion

//---------------------------------------------------------------------------------------------------------------------------------------------------