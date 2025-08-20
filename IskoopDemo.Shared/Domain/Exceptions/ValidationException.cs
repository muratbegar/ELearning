using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using IskoopDemo.Shared.Application.DTOs;

namespace IskoopDemo.Shared.Domain.Exceptions
{
    public class ValidationException : BaseException
    {
        public List<ValidationError> Errors { get; private set; }

        public ValidationException(string message = "Validation failed")
            : base(message, "VALIDATION_ERROR")
        {
            Errors = new List<ValidationError>();
        }


        public ValidationException(IEnumerable<ValidationError> errors, string message = "Validation failed")
             : base(message, "VALIDATION_ERROR")
        {
            Errors = errors?.ToList() ?? new List<ValidationError>();
        }

        public ValidationException(string field, string errorMessage)
            : base($"Validation failed for field '{field}': {errorMessage}", "VALIDATION_ERROR")
        {
            Errors = new List<ValidationError> { new ValidationError(field, errorMessage) };
        }

        protected ValidationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            // Note: For production, you might want to implement proper serialization for Errors collection
        }

        public ValidationException AddError(string field, string message, string code = null, object attemptedValue = null)
        {
            Errors.Add(new ValidationError(field, message, code, attemptedValue));
            return this;
        }

        public ValidationException AddErrors(IEnumerable<ValidationError> errors)
        {
            Errors.AddRange(errors);
            return this;
        }

        public bool HasErrors => Errors?.Any() == true;
        public bool HasErrorForField(string field) => Errors?.Any(e => e.Field == field) == true;
        public IEnumerable<string> GetErrorsForField(string field) =>
            Errors?.Where(e => e.Field == field).Select(e => e.Message) ?? Enumerable.Empty<string>();

        // Factory methods
        public static ValidationException For(string field, string message, object attemptedValue = null)
        {
            return new ValidationException().AddError(field, message, attemptedValue: attemptedValue);
        }

        public static ValidationException Required(string field)
        {
            return For(field, $"The {field} field is required.", "REQUIRED");
        }

        public static ValidationException InvalidFormat(string field, string expectedFormat, object attemptedValue = null)
        {
            return For(field, $"The {field} field has an invalid format. Expected: {expectedFormat}", attemptedValue)
                .AddError(field, "", "INVALID_FORMAT", attemptedValue) as ValidationException;
        }

        public static ValidationException OutOfRange(string field, object min, object max, object attemptedValue = null)
        {
            return For(field, $"The {field} field must be between {min} and {max}.", attemptedValue)
                .AddError(field, "", "OUT_OF_RANGE", attemptedValue) as ValidationException;
        }
    }
}
