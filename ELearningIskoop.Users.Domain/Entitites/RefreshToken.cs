using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ELearningIskoop.BuildingBlocks.Domain;

namespace ELearningIskoop.Users.Domain.Entitites
{
    public class RefreshToken : BaseEntity
    {
        protected RefreshToken() {}

        private RefreshToken(int userId, string token, string createdByIp, int daysToExpire)
        {
            UserId = userId;
            Token = token;
            ExpiresAt = DateTime.UtcNow.AddDays(daysToExpire);
            CreatedByIp = createdByIp;
        }

        public int UserId { get; private set; }
        public string Token { get; private set; } = string.Empty;
        public DateTime ExpiresAt { get; private set; }
        public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
        public DateTime? RevokedAt { get; private set; }
        public string CreatedByIp { get; private set; } = string.Empty;
        public string? RevokedByIp { get; private set; }
        public string? RevokedReason { get; private set; }
        public string? ReplacedByToken { get;private set; }

        public bool IsActive => RevokedAt == null && !IsExpired;
        public bool IsRevoked => RevokedAt != null;

        //Navigation
        public User User { get; private set; } = null;

        public static RefreshToken Create(int userId, string token, string ipAddress, int daysToExpire = 7)
        {
            return new RefreshToken(userId, token, ipAddress, daysToExpire);
        }

        public void Revoke(string reason, string ipAddress, string replacedByToken = null)
        {
            RevokedAt = DateTime.UtcNow;
            RevokedReason = reason;
            RevokedByIp = ipAddress;
            ReplacedByToken = replacedByToken;
        }

    }
}
