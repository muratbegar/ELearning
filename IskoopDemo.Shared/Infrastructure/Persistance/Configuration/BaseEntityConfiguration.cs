using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Linq.Expressions;
using IskoopDemo.Shared.Infrastructure.Common;

namespace IskoopDemo.Shared.Infrastructure.Persistance.Configuration
{
    public abstract class BaseEntityConfiguration<T> : IEntityTypeConfiguration<T> where T : BaseEntity
    {
        public void Configure(EntityTypeBuilder<T> builder)
        {
            builder.HasKey(e => e.ObjectId);

            builder.Property(x => x.CreatedAt).IsRequired();

            builder.Property(x => x.UpdatedAt);
            builder.Property(x => x.CreatedBy)
                .IsRequired()
                .HasMaxLength(256);
            builder.Property(e => e.UpdatedBy)
                .HasMaxLength(50);

            // Soft delete
            builder.Property(e => e.IsDeleted)
                .IsRequired();


            builder.Property(e => e.DeletedAt);

            builder.Property(e => e.DeletedBy)
                .HasMaxLength(50);

            // Global query filter for soft delete
            builder.HasQueryFilter(e => !e.IsDeleted);

            // Indexes
            builder.HasIndex(e => e.CreatedAt);
            builder.HasIndex(e => e.IsDeleted);

        }
    }
}
