using ELearningIskoop.Users.Domain.Entitites;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ELearningIskoop.Users.Infrastructure.Persistence.Configurations
{
    public class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
    {
        public void Configure(EntityTypeBuilder<RolePermission> builder)
        {
            builder.ToTable("RolePermissions");
            builder.HasKey(rp => rp.ObjectId);
            builder.Property(rp => rp.ObjectId).ValueGeneratedOnAdd();

            builder.Property(rp => rp.Permission)
                .IsRequired()
                .HasMaxLength(200);

            builder.HasIndex(rp => new { rp.RoleId, rp.Permission })
                .IsUnique()
                .HasDatabaseName("IX_RolePermissions_RoleId_Permission");
        }
    }
}
