using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IskoopDemo.Shared.Middleware.ErrorHandling
{
    public class ForbiddenException : Exception
    {
        public string Permission { get; }

        public ForbiddenException(string message = "Access forbidden", string permission = null)
            : base(message)
        {
            Permission = permission;
        }
    }
}
