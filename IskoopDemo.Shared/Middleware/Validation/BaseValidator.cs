using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IskoopDemo.Shared.Middleware.Validation
{
    public abstract class BaseValidator<T> : AbstractValidator<T> where T : class
    {
        protected void ValidateId(string id)
        {
            RuleFor(x => id)
                .NotEmpty().WithMessage("Id is required")
                .Must(BeValidGuid).WithMessage("Id must be a valid GUID");
        }

        protected bool BeValidGuid(string id)
        {
            return Guid.TryParse(id, out _);
        }

        protected bool BeValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        protected bool BeValidPhoneNumber(string phoneNumber)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(phoneNumber, @"^\+?[1-9]\d{1,14}$");
        }
    }
}
