using ELearningIskoop.BuildingBlocks.Domain.Repositories;
using ELearningIskoop.Courses.Domain.Entities;
using ELearningIskoop.Courses.Domain.Repositories;
using ELearningIskoop.Courses.Infrastructure.Persistence;
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
    public class CategoryRepository : ICategoryRepository
    {
        private readonly CoursesDbContext _context;

        public CategoryRepository(CoursesDbContext context)
        {
            _context = context;
        }


        public async Task<Category?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.categories
                .Include(c => c.SubCategories)
                .Include(c => c.ParentCategory)
                .FirstOrDefaultAsync(c => c.ObjectId == id, cancellationToken);
        }

        public async Task<IEnumerable<Category>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.categories
                .Include(c => c.SubCategories)
                .Include(c => c.ParentCategory)
                .OrderBy(c => c.DisplayOrder)
                .ThenBy(c => c.Name)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Category>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            return await _context.categories
                .Include(c => c.SubCategories)
                .Include(c => c.ParentCategory)
                .OrderBy(c => c.DisplayOrder)
                .ThenBy(c => c.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Category>> FindAsync(Expression<Func<Category, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await _context.categories
                .Include(c => c.SubCategories)
                .Include(c => c.ParentCategory)
                .Where(predicate)
                .ToListAsync(cancellationToken);
        }

        public async Task<Category> GetSingleAsync(Expression<Func<Category, bool>> predicate, CancellationToken cancellationToken = default)
        {
            var category = await _context.categories
                .Include(c => c.SubCategories)
                .Include(c => c.ParentCategory)
                .FirstOrDefaultAsync(predicate, cancellationToken);

            if (category == null)
                throw new EntityNotFoundException("Category", "predicate");

            return category;
        }

        public async Task<bool> ExistsAsync(Expression<Func<Category, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await _context.categories.AnyAsync(predicate, cancellationToken);
        }

        public async Task<int> CountAsync(CancellationToken cancellationToken = default)
        {
            return await _context.categories.CountAsync(cancellationToken);
        }

        public async Task<int> CountAsync(Expression<Func<Category, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await _context.categories.CountAsync(predicate, cancellationToken);
        }

        public async Task AddAsync(Category entity, CancellationToken cancellationToken = default)
        {
            _context.categories.Add(entity);
        }

        public async Task AddRangeAsync(IEnumerable<Category> entities, CancellationToken cancellationToken = default)
        {
            _context.categories.AddRange(entities);
        }

        public async Task UpdateAsync(Category entity, CancellationToken cancellationToken = default)
        {
            _context.categories.Update(entity);
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var category = await GetByIdAsync(id, cancellationToken);
            if (category != null)
            {
                _context.categories.Remove(category);
            }
        }

        public async Task DeleteAsync(Category entity, CancellationToken cancellationToken = default)
        {
            _context.categories.Remove(entity);
        }

        public  async Task DeleteRangeAsync(IEnumerable<Category> entities, CancellationToken cancellationToken = default)
        {
            _context.categories.RemoveRange(entities);
        }

        public async Task<IEnumerable<Category>> GetActiveAsync(CancellationToken cancellationToken = default)
        {
            return await _context.categories
                .Include(c => c.SubCategories.Where(sc => sc.IsActive))
                .Include(c => c.ParentCategory)
                .Where(c => c.IsActive)
                .OrderBy(c => c.DisplayOrder)
                .ThenBy(c => c.Name)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Category>> GetDeletedAsync(CancellationToken cancellationToken = default)
        {
            return await _context.categories
                .IgnoreQueryFilters()
                .Where(c => c.IsDeleted)
                .Include(c => c.SubCategories)
                .Include(c => c.ParentCategory)
                .ToListAsync(cancellationToken);
        }

        public async Task<Category?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
        {
            return await _context.categories
                .Include(c => c.SubCategories.Where(sc => sc.IsActive))
                .Include(c => c.ParentCategory)
                .FirstOrDefaultAsync(c => c.Slug == slug, cancellationToken);
        }

        public Task<IEnumerable<Category>> GetActiveCategoriesAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Category>> GetRootCategoriesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.categories
                .Include(c => c.SubCategories.Where(sc => sc.IsActive))
                .Where(c => c.ParentCategoryId == null && c.IsActive)
                .OrderBy(c => c.DisplayOrder)
                .ThenBy(c => c.Name)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Category>> GetSubCategoriesAsync(int parentCategoryId, CancellationToken cancellationToken = default)
        {
            return await _context.categories
                .Where(c => c.ParentCategoryId == parentCategoryId && c.IsActive)
                .OrderBy(c => c.DisplayOrder)
                .ThenBy(c => c.Name)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Category>> GetCategoryHierarchyAsync(CancellationToken cancellationToken = default)
        {
            // Tüm kategorileri yükle ve memory'de hiyerarşi oluştur
            return await _context.categories
                .Include(c => c.SubCategories)
                .Include(c => c.ParentCategory)
                .Where(c => c.IsActive)
                .OrderBy(c => c.ParentCategoryId.HasValue ? 1 : 0) // Root'lar önce
                .ThenBy(c => c.DisplayOrder)
                .ThenBy(c => c.Name)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<CategoryWithCourseCount>> GetWithCourseCountAsync(CancellationToken cancellationToken = default)
        {
            var result = await _context.categories
                .Where(c => c.IsActive)
                .Select(c => new CategoryWithCourseCount(
                    c.ObjectId,
                    c.Name,
                    c.Slug,
                    c.Courses.Count(cc => cc.Course.Status == Domain.Enums.CourseStatus.Published),
                    c.IsActive))
                .OrderByDescending(c => c.CourseCount)
                .ThenBy(c => c.CategoryName)
                .ToListAsync(cancellationToken);

            return result;
        }
    }
    
}
