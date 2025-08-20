using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IskoopDemo.Shared.Middleware.ErrorHandling
{
    public class ConflictException : Exception
    {
        public string ConflictType { get; }
        public object ConflictingValue { get; }

        public ConflictException(string message, string conflictType = null, object conflictingValue = null)
            : base(message)
        {
            ConflictType = conflictType;
            ConflictingValue = conflictingValue;
        }
    }
}
