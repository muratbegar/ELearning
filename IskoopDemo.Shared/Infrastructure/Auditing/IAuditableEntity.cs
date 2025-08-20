using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IskoopDemo.Shared.Infrastructure.Auditing
{
    public interface IAuditableEntity
    {
        DateTime CreatedAt { get; set; }
        int CreatedBy { get; set; }
        DateTime? UpdatedAt { get; set; }
        int? UpdatedBy { get; set; }

        void SetCreatedInfo(int createdBy);
        void SetUpdatedInfo(int updatedBy);
    }
}
