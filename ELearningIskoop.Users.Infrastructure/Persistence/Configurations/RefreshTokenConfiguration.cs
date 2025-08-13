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
    public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.ToTable("RefreshToken");
            builder.HasKey(x => x.ObjectId);
            builder.Property(x => x.ObjectId).ValueGeneratedOnAdd();


            builder.Property(x => x.Token).IsRequired().HasMaxLength(500);
            builder.Property(x => x.CreatedByIp).IsRequired().HasMaxLength(45);
            builder.Property(x => x.RevokedByIp).HasMaxLength(45);
            builder.Property(x => x.RevokedReason).HasMaxLength(500);
            builder.Property(x => x.ReplacedByToken).HasMaxLength(500);

            builder.HasIndex(rt => rt.Token)
                .IsUnique()
                .HasDatabaseName("IX_RefreshTokens_Token");

            builder.HasIndex(rt => new { rt.UserId, rt.ExpiresAt })
                .HasDatabaseName("IX_RefreshTokens_UserId_ExpiresAt");

        }
    }
}
