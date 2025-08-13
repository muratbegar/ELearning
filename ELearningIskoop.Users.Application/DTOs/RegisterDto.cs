using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELearningIskoop.Users.Application.DTOs
{
    public record RegisterDto(
        string Email,
        string FirstName,
        string LastName,
        string Password);
}
