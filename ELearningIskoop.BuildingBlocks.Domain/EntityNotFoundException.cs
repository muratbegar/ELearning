using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELearningIskoop.BuildingBlocks.Domain
{
    public class EntityNotFoundException : DomainException
    {
        public EntityNotFoundException(string entityName, object id)
            : base($"{entityName} entity with id {id} was not found.", "ENTITY_NOT_FOUND")
        {
        }
    }
}
