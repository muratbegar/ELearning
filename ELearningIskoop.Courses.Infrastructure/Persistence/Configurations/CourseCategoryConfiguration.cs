using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ELearningIskoop.Courses.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ELearningIskoop.Courses.Infrastructure.Persistence.Configurations
{
    public class CourseCategoryConfiguration : IEntityTypeConfiguration<CourseCategory>
    {
        public void Configure(EntityTypeBuilder<CourseCategory> builder)
        {
            // Table
            builder.ToTable("CourseCategories", "courses");
            // Primary Key
            builder.HasKey(cc => cc.ObjectId);

            #region Properties

            builder.Property(cc => cc.ObjectId)
                .ValueGeneratedOnAdd();

            builder.Property(cc => cc.CourseId)
                .IsRequired();

            builder.Property(cc => cc.CategoryId)
                .IsRequired();

            builder.Property(cc => cc.CreatedAt)
                .IsRequired();

            #endregion

            #region Relationships

            builder.HasOne(cc => cc.Course)
                .WithMany(c => c.Categories)
                .HasForeignKey(cc => cc.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(cc => cc.Category)
                .WithMany(c => c.Courses)
                .HasForeignKey(cc => cc.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);

            #endregion

            #region Indexes

            builder.HasIndex(cc => new { cc.CourseId, cc.CategoryId })
                .IsUnique()
                .HasDatabaseName("IX_CourseCategories_CourseId_CategoryId");

            builder.HasIndex(cc => cc.CategoryId)
                .HasDatabaseName("IX_CourseCategories_CategoryId");

            #endregion
        }
    }
}
