using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ELearningIskoop.BuildingBlocks.Domain;
using ELearningIskoop.BuildingBlocks.Domain.Repositories;
using ELearningIskoop.Courses.Domain.Entities;

namespace ELearningIskoop.Courses.Domain.Repositories
{
    public interface ICategoryRepository : IRepository<Category>
    {
        // Slug'a göre kategori getirir
        Task<Category?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);

        //Aktif kategorileri getirir
        Task<IEnumerable<Category>> GetActiveCategoriesAsync(CancellationToken cancellationToken = default);

        //Kategorileri ana kategoriye göre getirir
        Task<IEnumerable<Category>> GetRootCategoriesAsync(CancellationToken cancellationToken = default);

        //Kategorileri alt kategoriye göre getirir
        Task<IEnumerable<Category>> GetSubCategoriesAsync(int parentCategoryId, CancellationToken cancellationToken = default);

        // Kategori hiyerarşisini getirir
        Task<IEnumerable<Category>> GetCategoryHierarchyAsync(CancellationToken cancellationToken = default);

        // Kategori ile birlikte kurs sayısını getirir
        Task<IEnumerable<CategoryWithCourseCount>> GetWithCourseCountAsync(CancellationToken cancellationToken = default);
    }
}
