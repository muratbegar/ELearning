using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ELearningIskoop.BuildingBlocks.Domain;
using ELearningIskoop.Courses.Domain.Events;

namespace ELearningIskoop.Courses.Domain.Entities
{
    public class Category : BaseEntity, IAuditableEntity
    {
        protected Category() { } // EF Core için
        private readonly List<CourseCategory> _courses = new();

        private Category(string name, string description,string slug)
        {
            Name = name;
            Description = description;
            Slug = slug;
            IsActive = true;
            CreatedAt = DateTime.UtcNow;
        }

        public string Name { get; private set; } = string.Empty;
        public string Description { get; private set; } = string.Empty;
        public string Slug { get; private set; } = string.Empty;
        public bool IsActive { get; private set; } = true;
        public string? IconUrl { get; private set; }
        public string? Color { get; private set; } // Hex color code
        public int DisplayOrder { get; private set; } = 0;

        public int? ParentCategoryId { get; private set; }
        public Category? ParentCategory { get; private set; }
        public ICollection<Category> SubCategories { get; private set; } = new List<Category>();

        public IReadOnlyCollection<CourseCategory> Courses => _courses.AsReadOnly();


        // Yeni kategori oluşturucu metod
        public static Category Create(string name, string description, int? createdBy = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("Kategori adı boş olamaz", "CATEGORY_NAME_REQUIRED");

            if (name.Length > 100)
                throw new DomainException("Kategori adı 100 karakterden uzun olamaz", "CATEGORY_NAME_TOO_LONG");

            // Slug oluştur (URL-friendly)
            var slug = GenerateSlug(name);

            var category = new Category(name, description, slug);
            category.CreatedBy = createdBy;

            category.AddDomainEvent(new CategoryCreatedDomainEvent(category.ObjectId, category.Name, category.Slug));

            return category;
        }

        // Kategori bilgilerini güncelle
        public void UpdateDetails(string name, string description, int? updatedBy = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("Kategori adı boş olamaz", "CATEGORY_NAME_REQUIRED");

            Name = name;
            Description = description;
            Slug = GenerateSlug(name); // Slug'ı güncelle
            SetUpdatedInfo(updatedBy);
            AddDomainEvent(new CategoryUpdatedDomainEvent(ObjectId, Name));
        }

        // Alt kategori ekle
        public void AddSubCategory(Category subCategory, int? updatedBy = null)
        {
            if (subCategory.ParentCategoryId.HasValue)
                throw new BusinessRuleViolationException("CATEGORY_PARENT", "Bu kategori zaten bir üst kategoriye sahip");

            if (subCategory.ObjectId == ObjectId)
                throw new BusinessRuleViolationException("CATEGORY_SELF_PARENT", "Kategori kendisinin alt kategorisi olamaz");

            subCategory.SetParentCategory(ObjectId);
            SubCategories.Add(subCategory);

            SetUpdatedInfo(updatedBy);
        }

        // Üst kategoriyi ayarla
        public void SetParentCategory(int? parentCategoryId)
        {
            ParentCategoryId = parentCategoryId;
        }

        // İkon URL'sini ayarla
        public void SetIcon(string iconUrl, int? updatedBy = null)
        {
            IconUrl = iconUrl;
            SetUpdatedInfo(updatedBy);
        }

        // Renk kodunu ayarla
        public void SetColor(string colorHex, int? updatedBy = null)
        {
            // Basit hex color validation
            if (!string.IsNullOrEmpty(colorHex) && !colorHex.StartsWith("#"))
                colorHex = "#" + colorHex;

            Color = colorHex;
            SetUpdatedInfo(updatedBy);
        }

        // Görüntüleme sırasını ayarla
        public void SetDisplayOrder(int order, int? updatedBy = null)
        {
            DisplayOrder = order;
            SetUpdatedInfo(updatedBy);
        }


        // Kategoriyi aktif/pasif yap
        public void SetActiveStatus(bool isActive, int? updatedBy = null)
        {
            IsActive = isActive;
            SetUpdatedInfo(updatedBy);

            var eventType = isActive ? "activated" : "deactivated";
            AddDomainEvent(new CategoryStatusChangedDomainEvent(ObjectId, Name, isActive));
        }


        // Alt kategorileri var mı?
        public bool HasSubCategories => SubCategories.Any();

        // Kök kategori mi?
        public bool IsRootCategory => !ParentCategoryId.HasValue;

        // Private helper methods
        private static string GenerateSlug(string name)
        {
            return name
                .ToLowerInvariant()
                .Replace(" ", "-")
                .Replace("ç", "c")
                .Replace("ğ", "g")
                .Replace("ı", "i")
                .Replace("ö", "o")
                .Replace("ş", "s")
                .Replace("ü", "u")
                .Replace("İ", "i")
                .Replace("Ç", "c")
                .Replace("Ğ", "g")
                .Replace("Ö", "o")
                .Replace("Ş", "s")
                .Replace("Ü", "u")
                // Remove special characters
                .Where(c => char.IsLetterOrDigit(c) || c == '-')
                .Aggregate("", (current, c) => current + c);
        }
    }
}
