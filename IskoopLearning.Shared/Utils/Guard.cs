using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IskoopLearning.Shared.Utils
{
    public static class Guard
    {
        public static void AgainstNull(object value, string paramName)
        {
            if (value == null)
                throw new ArgumentNullException(paramName);
        }

        public static void AgainstEmpty(string value, string paramName)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Value cannot be empty", paramName);
        }

        public static void AgainstNegative(decimal value, string paramName)
        {
            if (value < 0)
                throw new ArgumentException("Value cannot be negative", paramName);
        }
    }
}
