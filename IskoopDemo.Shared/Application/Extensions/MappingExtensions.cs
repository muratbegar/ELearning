using IskoopDemo.Shared.Application.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace IskoopDemo.Shared.Application.Extensions
{
    public static class MappingExtensions
    {
        public static TDestination MapTo<TDestination>(this object source) where TDestination : new()
        {
            if (source == null) return default;

            var destination = new TDestination();
            var sourceType = source.GetType();
            var destinationType = typeof(TDestination);

            var sourceProperties = sourceType.GetProperties();
            var destinationProperties = destinationType.GetProperties();

            foreach (var destProp in destinationProperties.Where(p => p.CanWrite))
            {
                var sourceProp = sourceProperties.FirstOrDefault(p =>
                    p.Name.Equals(destProp.Name, StringComparison.OrdinalIgnoreCase) && p.CanRead);

                if (sourceProp != null && IsCompatibleType(sourceProp.PropertyType, destProp.PropertyType))
                {
                    var value = sourceProp.GetValue(source);
                    if (value != null)
                    {
                        destProp.SetValue(destination, ConvertValue(value, destProp.PropertyType));
                    }
                }
            }

            return destination;
        }

        public static TDestination MapTo<TDestination>(this object source, TDestination destination)
        {
            if (source == null || destination == null) return destination;

            var sourceType = source.GetType();
            var destinationType = typeof(TDestination);

            var sourceProperties = sourceType.GetProperties();
            var destinationProperties = destinationType.GetProperties();

            foreach (var destProp in destinationProperties.Where(p => p.CanWrite))
            {
                var sourceProp = sourceProperties.FirstOrDefault(p =>
                    p.Name.Equals(destProp.Name, StringComparison.OrdinalIgnoreCase) && p.CanRead);

                if (sourceProp != null && IsCompatibleType(sourceProp.PropertyType, destProp.PropertyType))
                {
                    var value = sourceProp.GetValue(source);
                    if (value != null)
                    {
                        destProp.SetValue(destination, ConvertValue(value, destProp.PropertyType));
                    }
                }
            }

            return destination;
        }

        public static IEnumerable<TDestination> MapToList<TDestination>(this IEnumerable<object> source)
            where TDestination : new()
        {
            return source?.Select(item => item.MapTo<TDestination>()) ?? Enumerable.Empty<TDestination>();
        }

        public static Dictionary<string, object> ToDictionary(this object obj)
        {
            if (obj == null) return new Dictionary<string, object>();

            return obj.GetType()
                     .GetProperties()
                     .Where(p => p.CanRead)
                     .ToDictionary(
                         prop => prop.Name,
                         prop => prop.GetValue(obj)
                     );
        }

        public static T FromDictionary<T>(this Dictionary<string, object> dict) where T : new()
        {
            var obj = new T();
            var type = typeof(T);

            foreach (var kvp in dict)
            {
                var property = type.GetProperty(kvp.Key);
                if (property != null && property.CanWrite)
                {
                    var value = ConvertValue(kvp.Value, property.PropertyType);
                    property.SetValue(obj, value);
                }
            }

            return obj;
        }

        public static string ToJson(this object obj, JsonSerializerOptions options = null)
        {
            if (obj == null) return null;

            options ??= new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };

            return JsonSerializer.Serialize(obj, options);
        }

        public static T FromJson<T>(this string json, JsonSerializerOptions options = null)
        {
            if (json.IsNullOrWhiteSpace()) return default;

            options ??= new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true
            };

            try
            {
                return JsonSerializer.Deserialize<T>(json, options);
            }
            catch
            {
                return default;
            }
        }

        private static bool IsCompatibleType(Type sourceType, Type destinationType)
        {
            if (sourceType == destinationType) return true;

            var destType = Nullable.GetUnderlyingType(destinationType) ?? destinationType;
            var srcType = Nullable.GetUnderlyingType(sourceType) ?? sourceType;

            return destType.IsAssignableFrom(srcType) ||
                   HasImplicitConversion(srcType, destType);
        }

        private static bool HasImplicitConversion(Type from, Type to)
        {
            try
            {
                var expr = Expression.Convert(Expression.Parameter(from), to);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static object ConvertValue(object value, Type targetType)
        {
            if (value == null) return null;
            if (targetType.IsAssignableFrom(value.GetType())) return value;

            var underlyingType = Nullable.GetUnderlyingType(targetType) ?? targetType;

            try
            {
                if (underlyingType.IsEnum)
                {
                    return Enum.Parse(underlyingType, value.ToString(), true);
                }

                return Convert.ChangeType(value, underlyingType);
            }
            catch
            {
                return GetDefaultValue(targetType);
            }
        }

        private static object GetDefaultValue(Type type)
        {
            return type.IsValueType ? Activator.CreateInstance(type) : null;
        }
    }
}
