using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IskoopDemo.Shared.Application.Interfaces.Cryptography
{
    public interface IHashingService
    {
        string Hash(string input);
        string HashWithSalt(string input, out string salt);
        bool Verify(string input, string hash, string salt);
        string GenerateSalt();
        string ComputeHmac(string data, string key);
        bool VerifyHmac(string data, string key, string hmac);
    }
}
