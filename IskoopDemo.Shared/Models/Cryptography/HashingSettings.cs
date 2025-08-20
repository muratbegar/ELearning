using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace IskoopDemo.Shared.Models.Cryptography
{
    public class HashingSettings
    {
        public int SaltSize { get; set; } = 128;
        public int HashSize { get; set; } = 256;
        public int Iterations { get; set; } = 100000;
        public KeyDerivationPrf Algorithm { get; set; } = KeyDerivationPrf.HMACSHA256;
    }
}
