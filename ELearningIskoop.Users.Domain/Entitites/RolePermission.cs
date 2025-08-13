using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ELearningIskoop.BuildingBlocks.Domain;

namespace ELearningIskoop.Users.Domain.Entitites
{
    public class RolePermission : BaseEntity
    {
        protected RolePermission () { }

        private RolePermission(int roleId, string permission)
        {
            RoleId = roleId;
            Permission = permission;
            IsActive = true;
        }

        public int RoleId { get; private set; }
        public string Permission { get; private set; }
        public bool IsActive { get; private set; }

        public Role Role { get; private set; }

        public static RolePermission Create(int roleId, string permission)
        {
            return new RolePermission(roleId, permission);
        }



    }
}
