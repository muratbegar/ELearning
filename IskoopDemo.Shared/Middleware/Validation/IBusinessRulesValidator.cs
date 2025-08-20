using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IskoopDemo.Shared.Middleware.Validation
{
    public interface IBusinessRulesValidator
    {
        Task<BusinessValidationResult> ValidateAsync(BusinessValidationContext context);
    }

}
