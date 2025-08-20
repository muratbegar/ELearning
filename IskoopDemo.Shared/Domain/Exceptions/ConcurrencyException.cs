using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IskoopDemo.Shared.Domain.Exceptions
{
    public class ConcurrencyException : DomainException
    {
        public string EntityId { get; private set; }
        public string EntityType { get; private set; }
        public string ExpectedVersion { get; private set; }
        public string ActualVersion { get; private set; }

        public ConcurrencyException(string message = "A concurrency conflict occurred")
            : base(message, "CONCURRENCY_CONFLICT")
        {
        }

        public ConcurrencyException(string entityId, string entityType, string expectedVersion, string actualVersion)
            : base($"Concurrency conflict for {entityType} '{entityId}'. Expected version: {expectedVersion}, Actual version: {actualVersion}", "CONCURRENCY_CONFLICT")
        {
            EntityId = entityId;
            EntityType = entityType;
            ExpectedVersion = expectedVersion;
            ActualVersion = actualVersion;
        }
    }
}
