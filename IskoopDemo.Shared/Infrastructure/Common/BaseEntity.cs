using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IskoopDemo.Shared.Infrastructure.Common
{
    public abstract class BaseEntity
    {
        public int ObjectId { get; protected set; }
        public DateTime CreatedAt { get; protected set; }
        public DateTime? UpdatedAt { get; protected set; }

        public string CreatedBy { get; protected set; }
        public string UpdatedBy { get; protected set; }
        public bool IsDeleted { get; protected set; } = false;
        public DateTime? DeletedAt { get; protected set; }
        public string DeletedBy { get; protected set; }

        public BaseEntity()
        {
            CreatedAt = DateTime.UtcNow;
            IsDeleted = false;
        }

        public void MarkAsDeleted(string deletedBy =null)
        {
            IsDeleted = true;
            DeletedAt = DateTime.UtcNow;
            DeletedBy = deletedBy;
        }

        public void UpdateTimeStamp(string updatedBy = null)
        {
            UpdatedAt = DateTime.UtcNow;
            UpdatedBy = updatedBy;
        }

        public override bool Equals(object? obj)
        {
            if (obj is not BaseEntity other)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            if (GetType() != other.GetType())
                return false;



            return ObjectId == other.ObjectId;
        }

        public override int GetHashCode()
        {
            return ObjectId.GetHashCode();
        }

        public static bool operator == (BaseEntity a, BaseEntity b)
        {
            if (a is null && b is null)
                return true;

            if (a is null || b is null)
                return false;

            return a.Equals(b);
        }

        public static bool operator !=(BaseEntity a, BaseEntity b)
        {
            return !(a == b);
        }
    }
}
