using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using IskoopDemo.Shared.Application.Extensions;
using IskoopDemo.Shared.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace IskoopDemo.Shared.Infrastructure.Persistance.Configuration
{
    public static class ValueObjectConfiguration
    {
        public static void ConfigureEmail<T>(EntityTypeBuilder<T> builder, string propertyName = "Email") where T : class
        {
            var converter = new ValueConverter<Domain.ValueObjects.Email, string>(
                v=> v.Value,
                v => new Domain.ValueObjects.Email(v)
            );

            builder.Property<Domain.ValueObjects.Email>(propertyName).HasConversion(converter).HasMaxLength(254) // RFC 5321 limit
                .IsRequired();

            builder.HasIndex(propertyName).IsUnique();
        }

        public static void ConfigurePhoneNumber<T> (EntityTypeBuilder<T> builder, string propertyName = "PhoneNumber") where T : class
        {
            var converter = new ValueConverter<Domain.ValueObjects.PhoneNumber, string>(
                v => v.Value,
                v => new Domain.ValueObjects.PhoneNumber(v,v)
            );

            builder.Property<Domain.ValueObjects.Email>(propertyName).HasConversion(converter)
                .HasMaxLength(20); // RFC 5321 limit
        }

        public static void ConfigureMoney<T>(EntityTypeBuilder<T> builder, string propertyName = "Price")
            where T : class
        {
            var converterMoneyCurrency = new ValueConverter<Money, string>(
            
                v=>v.Currency,
                v=> new Money(0,v) // Default amount is 0, currency is set from the string
                );

        }
    }
}
