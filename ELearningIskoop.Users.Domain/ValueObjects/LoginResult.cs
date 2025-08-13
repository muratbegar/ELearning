using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELearningIskoop.Users.Domain.ValueObjects
{
    public class LoginResult
    {
        public bool IsSuccess { get; init; }
        public string? Error { get; init; }

        public static LoginResult Success() => new() { IsSuccess = true };
        public static LoginResult Failed(string error) => new() { IsSuccess = false, Error = error };

    }
}
