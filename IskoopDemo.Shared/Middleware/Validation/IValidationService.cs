using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IskoopDemo.Shared.Middleware.Validation
{
    public interface IValidationService
    {
        Task<bool> ValidateAsync<T>(T model) where T : class;
    }
}
