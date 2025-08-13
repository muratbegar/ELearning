using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELearningIskoop.Users.Application.DTOs
{
    public record UserDTOs
    {
        public int Id { get; init; }
        public string Email { get; init; } = string.Empty;
        public string FirstName { get; init; } = string.Empty;
        public string LastName { get; init; } = string.Empty;
        public string FullName { get; init; } = string.Empty;
        public string Username { get; init; } = string.Empty;
        public string Status { get; init; } = string.Empty;
        public bool IsEmailVerified { get; init; }
        public DateTime? LastLoginAt { get; init; }
        public string? ProfilePictureUrl { get; init; }
        public List<string> Roles { get; init; } = new();
        public DateTime CreatedAt { get; init; }
    }
}
