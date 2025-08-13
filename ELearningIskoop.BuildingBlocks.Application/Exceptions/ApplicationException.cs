using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELearningIskoop.BuildingBlocks.Application.Exceptions
{
    // Application katmanında oluşan exception'lar için base sınıf
    public class ApplicationException : Exception
    {
        public string? ErrorCode { get; }
        public Dictionary<string, string[]>? Details { get; }

        public ApplicationException(string message) : base(message)
        {
        }

        public ApplicationException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public ApplicationException(string message, string errorCode) : base(message)
        {
            ErrorCode = errorCode;
        }

        public ApplicationException(string message, string errorCode, Dictionary<string, string[]> details)
            : base(message)
        {
            ErrorCode = errorCode;
            Details = details;
        }
    }
}
