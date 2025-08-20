using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IskoopDemo.Shared.Models.Cryptography
{
    public class EncryptionSettings
    {
        public string MasterKey { get; set; }
        public int KeySize { get; set; } = 256;
        public int BlockSize { get; set; } = 128;
        public int SaltSize { get; set; } = 128;
        public int Iterations { get; set; } = 10000;
        public string Algorithm { get; set; } = "AES";
    }
}
