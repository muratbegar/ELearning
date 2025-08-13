using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ELearningIskoop.Shared.Domain.ValueObjects;

namespace ELearningIskoop.Users.Application.DTOs
{
    public record CreateUserDto(
        Email Email,
        PersonName Name,
        string Password);
}
