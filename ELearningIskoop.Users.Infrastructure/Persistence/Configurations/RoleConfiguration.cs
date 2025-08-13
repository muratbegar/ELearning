using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ELearningIskoop.Users.Domain.Entitites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ELearningIskoop.Users.Infrastructure.Persistence.Configurations
{
    public class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.ToTable("Roles");

            builder.HasKey(x => x.ObjectId);

            builder.Property(x => x.ObjectId).ValueGeneratedOnAdd();

            builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
            builder.Property(x => x.NormalizedName).IsRequired().HasMaxLength(100);
            builder.Property(x => x.Description).IsRequired().HasMaxLength(500);

            builder.HasMany(x => x.UserRoles).WithOne(ur => ur.Role).HasForeignKey(rp => rp.RoleId);
            builder.HasMany(x => x.Permissions).WithOne(rp => rp.Role).HasForeignKey(rp => rp.RoleId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasIndex(x => x.NormalizedName).IsUnique().HasDatabaseName("IX_Roles_NormalizedName");
        }
    }
}
