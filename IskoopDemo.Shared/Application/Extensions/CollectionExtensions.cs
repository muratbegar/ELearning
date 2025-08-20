using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IskoopDemo.Shared.Application.Extensions
{
    public static class CollectionExtensions
    {
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> collection)
        {
            return collection == null || !collection.Any();
        }

        public static bool HasItems<T>(this IEnumerable<T> collection)
        {
            return collection != null && collection.Any();
        }

        public static IEnumerable<T> EmptyIfNull<T>(this IEnumerable<T> collection)
        {
            return collection ?? Enumerable.Empty<T>();
        }

        public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T> collection)
        {
            return collection?.Where(item => item != null) ?? Enumerable.Empty<T>();
        }

        public static void ForEach<T>(this IEnumerable<T> collection, Action<T> action)
        {
            if (collection != null && action != null)
            {
                foreach (var item in collection)
                {
                    action(item);
                }
            }
        }

        public static IEnumerable<T> DistinctBy<T, TKey>(this IEnumerable<T> collection, Func<T, TKey> keySelector)
        {
            var seenKeys = new HashSet<TKey>();
            return collection?.Where(item => seenKeys.Add(keySelector(item))) ?? Enumerable.Empty<T>();
        }

        public static string JoinString<T>(this IEnumerable<T> collection, string separator = ", ")
        {
            return collection != null ? string.Join(separator, collection) : string.Empty;
        }

        public static IEnumerable<IEnumerable<T>> Batch<T>(this IEnumerable<T> collection, int batchSize)
        {
            var batch = new List<T>(batchSize);

            foreach (var item in collection.EmptyIfNull())
            {
                batch.Add(item);

                if (batch.Count == batchSize)
                {
                    yield return batch;
                    batch = new List<T>(batchSize);
                }
            }

            if (batch.Count > 0)
            {
                yield return batch;
            }
        }
    }
}
