using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELeraningIskoop.ServiceDefaults.Configuration
{
    // Configuration helper'ları
    public static class ConfigurationExtensions
    {
        // Connection string'i alır, bulamazsa exception fırlatır
        public static string GetRequiredConnectionString(this IConfiguration configuration, string name)
        {
            var connectionString = configuration.GetConnectionString(name);

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException($"Connection string '{name}' is required but not found.");
            }

            return connectionString;
        }

        // Application settings section'ını bind eder
        public static T GetRequiredSection<T>(this IConfiguration configuration, string sectionName)
            where T : class, new()
        {
            var section = configuration.GetSection(sectionName);

            if (!section.Exists())
            {
                throw new InvalidOperationException($"Configuration section '{sectionName}' is required but not found.");
            }

            var settings = new T();
            section.Bind(settings);

            return settings;
        }

        // Environment'a göre configuration değeri alır
        public static string GetEnvironmentValue(this IConfiguration configuration, string key, string defaultValue = "")
        {
            var value = configuration[key];
            return string.IsNullOrEmpty(value) ? defaultValue : value;
        }
    }
}
