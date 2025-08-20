using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IskoopDemo.Shared.Application.DTOs
{
    public class ValidationError
    {
        public string Field { get; set; }
        public string Message { get; set; }
        public string Code { get; set; }
        public object AttemptedValue { get; set; }

        public ValidationError() { }

        public ValidationError(string field, string message)
        {
            Field = field;
            Message = message;
        }

        public ValidationError(string field, string message, string code) : this(field, message)
        {
            Code = code;
        }

        public ValidationError(string field, string message, string code, object attemptedValue)
            : this(field, message, code)
        {
            AttemptedValue = attemptedValue;
        }

        public override string ToString()
        {
            return string.IsNullOrWhiteSpace(Field) ? Message : $"{Field}: {Message}";
        }

    }
}
