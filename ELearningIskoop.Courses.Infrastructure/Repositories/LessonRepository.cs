using ELearningIskoop.Courses.Domain.Entities;
using ELearningIskoop.Courses.Domain.Repositories;
using ELearningIskoop.Courses.Infrastructure.Persistence;
using ELearningIskoop.Shared.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using ELearningIskoop.BuildingBlocks.Domain;
using Microsoft.EntityFrameworkCore;

namespace ELearningIskoop.Courses.Infrastructure.Repositories
{
    public class LessonRepository : ILessonRepository
    {
        private readonly CoursesDbContext _context;

        public LessonRepository(CoursesDbContext context)
        {
            _context = context;
        }


        public async Task AddAsync(Lesson entity, CancellationToken cancellationToken = default)
        {
            _context.Lessons.Add(entity);
        }

        public async Task AddRangeAsync(IEnumerable<Lesson> entities, CancellationToken cancellationToken = default)
        {
            _context.Lessons.AddRange(entities);
        }

        public async Task<int> CountAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Lessons.CountAsync(cancellationToken);
        }

        public async Task<int> CountAsync(Expression<Func<Lesson, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await _context.Lessons.CountAsync(predicate, cancellationToken);
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var lesson = await GetByIdAsync(id, cancellationToken);
            if (lesson != null)
            {
                _context.Lessons.Remove(lesson);
            }
        }

        public async Task DeleteAsync(Lesson entity, CancellationToken cancellationToken = default)
        {
            _context.Lessons.Remove(entity);
        }

        public async Task DeleteRangeAsync(IEnumerable<Lesson> entities, CancellationToken cancellationToken = default)
        {
            _context.Lessons.RemoveRange(entities);
        }

        public async Task<bool> ExistsAsync(Expression<Func<Lesson, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await _context.Lessons.AnyAsync(predicate, cancellationToken);
        }

        public async Task<IEnumerable<Lesson>> FindAsync(Expression<Func<Lesson, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await _context.Lessons
                .Include(l => l.Course)
                .Where(predicate)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Lesson>> GetActiveAsync(CancellationToken cancellationToken = default)
        {
            return await GetAllAsync(cancellationToken);
        }

        public async Task<IEnumerable<Lesson>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Lessons
                .Include(l => l.Course)
                .OrderBy(l => l.CourseId)
                .ThenBy(l => l.Order)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Lesson>> GetByContentTypeAsync(ContentType contentType, CancellationToken cancellationToken)
        {
            return await _context.Lessons
                .Include(l => l.Course)
                .Where(l => l.ContentType == contentType)
                .OrderBy(l => l.CourseId)
                .ThenBy(l => l.Order)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Lesson>> GetByCourseId(int courseId, CancellationToken cancellationToken)
        {
            return await _context.Lessons
                .Where(l => l.CourseId == courseId)
                .OrderBy(l => l.Order)
                .ToListAsync(cancellationToken);
        }

        public async Task<Lesson?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Lessons
                .Include(l => l.Course)
                .FirstOrDefaultAsync(l => l.ObjectId == id, cancellationToken);
        }

        public async Task<IEnumerable<Lesson>> GetDeletedAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Lessons
                .IgnoreQueryFilters()
                .Where(l => l.IsDeleted)
                .Include(l => l.Course)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Lesson>> GetFreePreviewLessonsAsync(int courseId, CancellationToken cancellationToken)
        {
            return await _context.Lessons
                .Where(l => l.CourseId == courseId && l.IsFree)
                .OrderBy(l => l.Order)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Lesson>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            return await _context.Lessons
                .Include(l => l.Course)
                .OrderBy(l => l.CourseId)
                .ThenBy(l => l.Order)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);
        }

        public async Task<Lesson> GetSingleAsync(Expression<Func<Lesson, bool>> predicate, CancellationToken cancellationToken = default)
        {
            var lesson = await _context.Lessons
                .Include(l => l.Course)
                .FirstOrDefaultAsync(predicate, cancellationToken);

            if (lesson == null)
                throw new EntityNotFoundException("Lesson", "predicate");

            return lesson;
        }

        public async Task UpdateAsync(Lesson entity, CancellationToken cancellationToken = default)
        {
            _context.Lessons.Update(entity);
        }
    }
}
