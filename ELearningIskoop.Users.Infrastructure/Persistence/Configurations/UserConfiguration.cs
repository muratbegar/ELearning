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
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");

            builder.HasKey(x => x.ObjectId);

            //Properties
            builder.Property(x => x.ObjectId).ValueGeneratedNever();

            builder.Property(x => x.Username).IsRequired().HasMaxLength(100);
            builder.Property(x => x.SecurityStamp).IsRequired().HasMaxLength(100);
            builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(20);
            builder.Property(x => x.ProfilePictureUrl).HasMaxLength(500);
            builder.Property(x => x.Bio).HasMaxLength(1000);
            builder.Property(x => x.PhoneNumber).HasMaxLength(20);


            builder.OwnsOne(c => c.Email, builder=>
            {
                builder.Property(p => p.Value)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("Email");

            });

            builder.OwnsOne(x => x.Name, nameBuilder =>
            {
                builder.Property(n => n.Name).IsRequired().HasMaxLength(200).HasColumnName("Name");
            });

            builder.OwnsOne(x => x.Password, passwordBuilder =>
            {
                passwordBuilder.Property(x => x.Hash).IsRequired().HasMaxLength(500).HasColumnName("PasswordHash");
                passwordBuilder.Property(x => x.Salt).IsRequired().HasMaxLength(500).HasColumnName("PasswordSalt");
            });

            //Relationship
            builder.HasMany(u => u.RefreshTokens).WithOne(rt => rt.User).HasForeignKey(ur => ur.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(u => u.UserRoles)
                .WithOne(ur => ur.User)
                .HasForeignKey(ur => ur.UserId)
                .OnDelete(DeleteBehavior.Cascade);


            builder.HasMany(u => u.LoginHistory)
                .WithOne(ul => ul.User)
                .HasForeignKey(ul => ul.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            builder.HasIndex(u => u.Email.Value)
                .IsUnique()
                .HasDatabaseName("IX_Users_Email");

            builder.HasIndex(u => u.Username)
                .IsUnique()
                .HasDatabaseName("IX_Users_Username");

            builder.HasIndex(u => u.Status)
                .HasDatabaseName("IX_Users_Status");

            builder.HasIndex(u => new { u.IsDeleted, u.Status })
                .HasDatabaseName("IX_Users_IsDeleted_Status");
        }
    }
}
