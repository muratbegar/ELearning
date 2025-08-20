using IskoopLearning.Shared.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IskoopLearning.Shared.ValueObjects
{
    public class Status : Enumeration
    {

        public static Status Active = new(1, nameof(Active));
        public static Status Inactive = new(2, nameof(Inactive));
        public static Status Pending = new(3, nameof(Pending));
        public static Status Suspended = new(4, nameof(Suspended));
        public static Status Deleted = new(5, nameof(Deleted));
        public static Status Draft = new(6, nameof(Draft));
        public static Status Published = new(7, nameof(Published));
        public static Status Archived = new(7, nameof(Archived));

        protected Status(int id, string name) : base(id, name)
        {
        }

        public bool IsActive() => this == Active;
        public bool IsInactive() => this == Inactive;
        public bool IsDeleted() => this == Deleted;
        public bool IsPending() => this == Pending;
        public bool IsPublished() => this == Published;
        public bool IsDraft() => this == Draft;


        public bool CanTransitionTo(Status newStatus)
        {
            return this switch
            {
                _ when this == Draft => newStatus == Published || newStatus == Deleted,
                _ when this == Pending => newStatus == Active || newStatus == Suspended || newStatus == Deleted,
                _ when this == Active => newStatus == Inactive || newStatus == Suspended || newStatus == Deleted,
                _ when this == Inactive => newStatus == Active || newStatus == Deleted,
                _ when this == Suspended => newStatus == Active || newStatus == Deleted,
                _ when this == Published => newStatus == Archived || newStatus == Deleted,
                _ => false
            };
        }

    }
}
