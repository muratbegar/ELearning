using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IskoopDemo.Shared.Middleware.ErrorHandling
{
    public class NotFoundException : Exception
    {
        public string ResourceType { get; }
        public object ResourceId { get; }

        public NotFoundException(string resourceType, object resourceId)
            : base($"{resourceType} with id '{resourceId}' was not found")
        {
            ResourceType = resourceType;
            ResourceId = resourceId;
        }
    }
}
