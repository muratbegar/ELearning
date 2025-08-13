using ELearningIskoop.Courses.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace ELearningIskoop.Courses.Infrastructure.Persistence.Configurations
{
    public class CourseConfiguration : IEntityTypeConfiguration<Course>
    {
        public void Configure(EntityTypeBuilder<Course> builder)
        {
            // Table
            builder.ToTable("Courses", "courses");
            //Primary Key
            builder.HasKey(c => c.ObjectId);

            #region Properties

            //Properties
            builder.Property(x => x.ObjectId).ValueGeneratedOnAdd();//domain de oluşturuyoruz

            builder.Property(x => x.Title).IsRequired().HasMaxLength(200);
            builder.Property(x => x.Description).IsRequired().HasMaxLength(2000);
            builder.Property(x => x.MaxStudents).IsRequired().HasDefaultValue(1000);
            builder.Property(x => x.CurrentStudentCount).IsRequired().HasDefaultValue(0);
            builder.Property(x => x.ThumbnailUrl).HasMaxLength(500);
            builder.Property(x => x.TrailerVideoUrl).HasMaxLength(500);
            builder.Property(x => x.Level).HasConversion<string>().HasMaxLength(20);
            builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(20);

            #endregion

            #region Value Objects

            //value objects
            builder.OwnsOne(c => c.InstructorName, instructorBuilder =>
            {
                instructorBuilder.Property(p => p.FirstName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("InstructorFirstName");

                instructorBuilder.Property(p => p.LastName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("InstructorLastName");
            });
            builder.OwnsOne(c => c.InstructorEmail, emailBuilder =>
            {
                emailBuilder.Property(e => e.Value)
                    .IsRequired()
                    .HasMaxLength(254)
                    .HasColumnName("InstructorEmail");
            });


            // Value Objects - Money
            builder.OwnsOne(c => c.Price, priceBuilder =>
            {
                priceBuilder.Property(m => m.Amount)
                    .IsRequired()
                    .HasColumnType("decimal(18,2)")
                    .HasColumnName("Price");

                priceBuilder.Property(m => m.Currency)
                    .IsRequired()
                    .HasMaxLength(3)
                    .HasColumnName("Currency");
            });

            builder.OwnsOne(c => c.TotalDuration, durationBuilder =>
            {
                durationBuilder.Property(d => d.TotalMinutes)
                    .IsRequired()
                    .HasColumnName("TotalDurationMinutes");
            });
            #endregion

            #region Audit Fields

            // Audit Fields
            builder.Property(c => c.CreatedAt)
                .IsRequired();

            builder.Property(c => c.UpdatedAt);

            builder.Property(c => c.CreatedBy)
                .HasMaxLength(100);

            builder.Property(c => c.UpdatedBy)
                .HasMaxLength(100);

            builder.Property(c => c.PublishedAt);


            #endregion

            // Soft Delete
            builder.Property(c => c.IsDeleted)
                .IsRequired()
                .HasDefaultValue(false);

            #region Relationships

            // Relationships
            builder.HasMany(c => c.Lessons)
                .WithOne(l => l.Course)
                .HasForeignKey(l => l.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(c => c.Categories)
                .WithOne(cc => cc.Course)
                .HasForeignKey(cc => cc.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            #endregion

            #region Indexes

            // Indexes
           
            builder.HasIndex(c => c.Status)
                .HasDatabaseName("IX_Courses_Status");

            builder.HasIndex(c => c.Level)
                .HasDatabaseName("IX_Courses_Level");

            builder.HasIndex(c => c.PublishedAt)
                .HasDatabaseName("IX_Courses_PublishedAt");

            builder.HasIndex(c => c.CreatedAt)
                .HasDatabaseName("IX_Courses_CreatedAt");

            builder.HasIndex(c => new { c.IsDeleted, c.Status })
                .HasDatabaseName("IX_Courses_IsDeleted_Status");

            #endregion

            // Full-text search için (PostgreSQL)
            builder.HasIndex(c => new { c.Title, c.Description })
                .HasDatabaseName("IX_Courses_FullText");
        }
    }
}
