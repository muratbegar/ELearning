using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IskoopDemo.Shared.Models.Security
{
    public class RefreshTokenEntity
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Token { get; set; }
        public int UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
        public bool IsRevoked { get; set; }
        public DateTime? RevokedAt { get; set; }
        public string RevokedBy { get; set; }
        public string ReplacedByToken { get; set; }
        public string IpAddress { get; set; }
        public string UserAgent { get; set; }
    }
}
