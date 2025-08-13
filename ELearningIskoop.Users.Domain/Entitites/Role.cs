using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ELearningIskoop.BuildingBlocks.Domain;
using ELearningIskoop.Shared.Domain.Enums;

namespace ELearningIskoop.Users.Domain.Entitites
{
    public class Role : BaseEntity
    {
        private readonly List<UserRole> _userRoles = new();
        private readonly List<RolePermission> _permissions = new();

        protected Role(){}

        private Role(string name, string description)
        {
            Name = name;
            NormalizedName = name.ToUpperInvariant();
            Description = description;
            IsActive = true;
        }

        public string Name { get; private set; } = string.Empty;
        public string NormalizedName { get; private set; } = string.Empty;
        public string Description { get; private set; } = string.Empty;
        public bool IsActive { get; private set; }

        public IReadOnlyCollection<UserRole> UserRoles => _userRoles.AsReadOnly();
        public IReadOnlyCollection<RolePermission> Permissions => _permissions.AsReadOnly();

        public static Role Create(string name, string description)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Role name cannot be empty");

            return new Role(name, description);

        }

        public void AddPermission(string permission)
        {
            if(_permissions.Any(p=>p.Permission == permission))
                return;

            _permissions.Add(RolePermission.Create(ObjectId,permission));
        }

        public bool HasPermission(string permission)
        {
            return _permissions.Any(p => p.Permission == permission && p.IsActive);
        }

    }
}
