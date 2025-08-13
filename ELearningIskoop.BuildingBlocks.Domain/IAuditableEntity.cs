using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELearningIskoop.BuildingBlocks.Domain
{
    // Audit edilebilir entity'ler için interface
    // Kim ne zaman oluşturdu/güncelledi bilgilerini tutar
    public interface IAuditableEntity
    {
        DateTime CreatedAt { get; set; }
        int? CreatedBy { get; set; }
        DateTime? UpdatedAt { get; set; }
        int? UpdatedBy { get; set; }
        DateTime? DeletedAt { get; set; }
        int? DeletedBy { get; set; }

    }

    // Soft delete edilebilir entity'ler için interface
    public interface ISoftDeletableEntity
    {
        bool IsDeleted { get; set; }
        void SoftDelete(int? deletedBy = null);
    }
}
