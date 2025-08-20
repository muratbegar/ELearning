using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using IskoopDemo.Shared.Application.Extensions;


namespace IskoopDemo.Shared.Domain.Exceptions
{
    public class DomainException : Exception
    {
        protected DomainException(string message) : base(message) { }
        protected DomainException(string message, Exception innerException) : base(message, innerException) { }

        public string AggregateId { get; private set; }
        public string AggregateType { get; private set; }

        public DomainException(string message, string errorCode = null) : base(message)
        {
            // You can store errorCode in a property if needed
        }

        public DomainException(string message, Exception innerException, string errorCode = null)
            : base(message, innerException)
        {
            // You can store errorCode in a property if needed
        }

        protected DomainException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            AggregateId = info.GetString(nameof(AggregateId));
            AggregateType = info.GetString(nameof(AggregateType));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue(nameof(AggregateId), AggregateId);
            info.AddValue(nameof(AggregateType), AggregateType);
        }

        public DomainException WithAggregate(string aggregateId, string aggregateType)
        {
            AggregateId = aggregateId;
            AggregateType = aggregateType;
            return this;
        }

        // Factory methods for common domain errors
        public static DomainException InvalidOperation(string message, string aggregateId = null, string aggregateType = null)
        {
            //Aggregate veya Entity üzerinde geçersiz bir işlem yapılmaya çalışıldığında fırlatılır


            return new DomainException(message, "INVALID_OPERATION")
                .WithAggregate(aggregateId, aggregateType) as DomainException;
        }

        public static DomainException InvalidState(string message, string currentState, string aggregateId = null)
        {
            //Aggregate veya Entity’nin geçerli olmayan bir durumda olması halinde kullanılır.



            return ((DomainException)new DomainException(message, "INVALID_STATE")
                    .WithDetail("CurrentState", currentState))
                .WithAggregate(aggregateId, "Entity");
        }

        public static DomainException ConcurrencyConflict(string message, string aggregateId = null)
        {
            //Aynı aggregate üzerinde eşzamanlı değişiklikler yapıldığında fırlatılır.

            return new DomainException(message, "CONCURRENCY_CONFLICT")
                .WithAggregate(aggregateId, "Entity");
        }

    }
}
