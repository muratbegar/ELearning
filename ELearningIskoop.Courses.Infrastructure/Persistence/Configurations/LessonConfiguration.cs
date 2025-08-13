using ELearningIskoop.Courses.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELearningIskoop.Courses.Infrastructure.Persistence.Configurations
{
    public class LessonConfiguration : IEntityTypeConfiguration<Lesson>
    {
        public void Configure(EntityTypeBuilder<Lesson> builder)
        {
            // Table
            builder.ToTable("Lessons", "courses");
            // Primary Key
            builder.HasKey(l => l.ObjectId);

            #region Properties  

            builder.Property(l => l.ObjectId)
                .ValueGeneratedOnAdd();

            builder.Property(l => l.CourseId)
                .IsRequired();

            builder.Property(l => l.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(l => l.Description)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(l => l.Order)
                .IsRequired();

            builder.Property(l => l.IsPublished)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(l => l.IsFree)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(l => l.VideoUrl)
                .HasMaxLength(500);

            builder.Property(l => l.DocumentUrl)
                .HasMaxLength(500);

            builder.Property(l => l.AudioUrl)
                .HasMaxLength(500);

            builder.Property(l => l.InteractiveContent)
                .HasColumnType("jsonb"); // PostgreSQL JSON column

            #endregion

            // Enum
            builder.Property(l => l.ContentType)
                .HasConversion<string>()
                .HasMaxLength(20);

            // Value Objects - Duration
            builder.ComplexProperty(l => l.Duration, durationBuilder =>
            {
                durationBuilder.Property(d => d.TotalMinutes)
                    .IsRequired()
                    .HasColumnName("DurationMinutes");
            });

            #region Audit Fields

            builder.Property(l => l.CreatedAt).IsRequired();
            builder.Property(l => l.UpdatedAt);
            builder.Property(l => l.CreatedBy).HasMaxLength(100);
            builder.Property(l => l.UpdatedBy).HasMaxLength(100);
            builder.Property(l => l.IsDeleted).IsRequired().HasDefaultValue(false);

            #endregion

            // Relationships
            builder.HasOne(l => l.Course)
                .WithMany(c => c.Lessons)
                .HasForeignKey(l => l.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            #region Indexes

            builder.HasIndex(l => l.CourseId)
                .HasDatabaseName("IX_Lessons_CourseId");

            builder.HasIndex(l => new { l.CourseId, l.Order })
                .IsUnique()
                .HasDatabaseName("IX_Lessons_CourseId_Order");

            builder.HasIndex(l => l.ContentType)
                .HasDatabaseName("IX_Lessons_ContentType");

            builder.HasIndex(l => new { l.IsPublished, l.IsFree })
                .HasDatabaseName("IX_Lessons_IsPublished_IsFree");

            #endregion
        }
    }
}
