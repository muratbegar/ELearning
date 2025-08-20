using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IskoopDemo.Shared.Domain.Exceptions;

namespace IskoopDemo.Shared.Infrastructure.Common
{
    public  abstract class Entity<TId> : BaseEntity where TId : struct
    {
        public new TId ObjectId { get; protected set; }

        protected Entity(TId id)
        {
            ObjectId = id;
        }

        protected Entity()
        {
            
        }

        protected static void CheckRule(IBusinessRule rule)
        {
            if (rule.IsBroken())
            {
                throw new BusinessRuleValidationException(rule);
            }
        }
    }
}
