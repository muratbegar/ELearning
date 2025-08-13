using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELearningIskoop.BuildingBlocks.Domain
{
    // Domain katmanındaki business rule ihlalleri için exception sınıfı
    public class DomainException : Exception
    {

        // Hata kodu (API error handling için)
        public string ErrorCode { get; }


        // Hata detayları (validation error'ları vs)
        public Dictionary<string, string[]>? Details { get; }

        public DomainException(string message) : base(message)
        {
        }

        public DomainException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public DomainException(string message, string errorCode) : base(message)
        {
            ErrorCode = errorCode;
        }
        public DomainException(string message, string errorCode, Dictionary<string, string[]> details)
            : base(message)
        {
            ErrorCode = errorCode;
            Details = details;
        }
    }
}
