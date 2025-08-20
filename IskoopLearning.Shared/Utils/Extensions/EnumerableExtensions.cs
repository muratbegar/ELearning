using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IskoopLearning.Shared.Utils.Extensions
{
    public static class EnumerableExtensions
    {
        public static bool IsEmpty<T>(this IEnumerable<T> source)
        {
            return source == null || !source.Any();
        }

        public static string JoinToString<T>(this IEnumerable<T> source, string separator = ", ")
        {
            return string.Join(separator, source);
        }
    }
}
