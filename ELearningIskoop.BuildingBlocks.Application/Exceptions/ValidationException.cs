using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELearningIskoop.BuildingBlocks.Application.Exceptions
{
    // Validation başarısız olduğunda fırlatılan exception
    internal class ValidationException : ApplicationException
    {

        public ValidationException(IEnumerable<FluentValidation.Results.ValidationFailure> failures)
            : base("Validation hatası oluştu", "VALIDATION_ERROR", FormatErrors(failures))
        {
        }

        private static Dictionary<string, string[]> FormatErrors(IEnumerable<FluentValidation.Results.ValidationFailure> failures)
        {
            return failures
                .GroupBy(x => x.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(x => x.ErrorMessage).ToArray()
                );
        }
    }
}
