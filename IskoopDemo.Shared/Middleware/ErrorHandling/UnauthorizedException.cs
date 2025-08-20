using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IskoopDemo.Shared.Middleware.ErrorHandling
{
    public class UnauthorizedException : Exception
    {
        public string Reason { get; }

        public UnauthorizedException(string message = "Unauthorized access", string reason = null)
            : base(message)
        {
            Reason = reason;
        }
    }
}
