using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELearningIskoop.BuildingBlocks.Domain.Repositories
{
    public class CategoryWithCourseCount : ValueObject
    {

        public int CategoryId { get; init; }
        public string CategoryName { get; init; }
        public string CategorySlug { get; init; }
        public int CourseCount { get; init; }
        public bool IsActive { get; init; }

        public CategoryWithCourseCount(
            int categoryId,
            string categoryName,
            string categorySlug,
            int courseCount,
            bool isActive)
        {
            CategoryId = categoryId;
            CategoryName = categoryName;
            CategorySlug = categorySlug;
            CourseCount = courseCount;
            IsActive = isActive;
        }

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return CategoryId;
            yield return CategoryName;
            yield return CategorySlug;
            yield return CourseCount;
            yield return IsActive;
        }
    }
}
