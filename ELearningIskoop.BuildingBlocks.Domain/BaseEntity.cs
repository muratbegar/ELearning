using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;


namespace ELearningIskoop.BuildingBlocks.Domain
{
    // Tüm domain entity'leri için temel sınıf

    public abstract class BaseEntity : IAuditableEntity, ISoftDeletableEntity
    {
        private readonly List<IDomainEvent> _domainEvents = new();

        /// <summary>
        /// Entity'nin benzersiz kimlik numarası
        /// </summary>
        public int ObjectId { get; protected set; }

        /// <summary>
        /// Kayıt oluşturulma tarihi
        /// </summary>
        public DateTime CreatedAt { get; protected set; } = DateTime.UtcNow;

        /// <summary>
        /// Son güncelleme tarihi
        /// </summary>
        public DateTime? UpdatedAt { get; protected set; }

        /// <summary>
        /// Kaydı oluşturan kullanıcı ID'si
        /// </summary>
        public int? CreatedBy { get; protected set; }

        /// <summary>
        /// Son güncelleyen kullanıcı ID'si
        /// </summary>
        public int? UpdatedBy { get; protected set; }

        /// <summary>
        /// Soft delete için - kayıt silinmiş mi?
        /// </summary>
        public bool IsDeleted { get; protected set; }

        /// <summary>
        /// Domain event'leri - Entity Framework tarafından izlenmez
        /// </summary>
        [NotMapped]
        public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        DateTime IAuditableEntity.CreatedAt { get => CreatedAt; set => CreatedAt = value; }
        int? IAuditableEntity.CreatedBy { get => CreatedBy; set => CreatedBy = value; }
        DateTime? IAuditableEntity.UpdatedAt { get => UpdatedAt; set => UpdatedAt = value; }
        int? IAuditableEntity.UpdatedBy { get => UpdatedBy; set => UpdatedBy = value; }
        private DateTime? _deletedAt;
        public DateTime? DeletedAt
        {
            get => _deletedAt;
            set => _deletedAt = value;
        }

        private int? _deletedBy;
        public int? DeletedBy
        {
            get => _deletedBy;
            set => _deletedBy = value;
        }

        bool ISoftDeletableEntity.IsDeleted { get => IsDeleted; set => IsDeleted = value; }

        /// <summary>
        /// Entity güncelleme bilgilerini set eder
        /// </summary>
        protected void SetUpdatedInfo(int? updatedBy = null)
        {
            UpdatedAt = DateTime.UtcNow;
            UpdatedBy = updatedBy;
        }

        /// <summary>
        /// Soft delete'i geri alır
        /// </summary>
        public virtual void RestoreFromSoftDelete(int? restoredBy = null)
        {
            IsDeleted = false;
            SetUpdatedInfo(restoredBy);
        }

        /// <summary>
        /// Domain event ekler
        /// </summary>
        protected void AddDomainEvent(IDomainEvent domainEvent)
        {
            _domainEvents.Add(domainEvent);
        }

        /// <summary>
        /// Domain event'leri temizler (genellikle event publish edilince çağrılır)
        /// </summary>
        public void ClearDomainEvents()
        {
            _domainEvents.Clear();
        }

        /// <summary>
        /// Entity'ler için eşitlik kontrolü - Id bazlı
        /// </summary>
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

        public void SoftDelete(int? deletedBy = null)
        {
            throw new NotImplementedException();
        }

        public static bool operator ==(BaseEntity? left, BaseEntity? right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(BaseEntity? left, BaseEntity? right)
        {
            return !Equals(left, right);
        }

    }
}
