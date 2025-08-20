using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IskoopDemo.Shared.Application.Interfaces.Cryptography
{
    public interface IEncryptionService
    {
        string Encrypt(string plainText);
        string Decrypt(string cipherText);
        byte[] Encrypt(byte[] data);
        byte[] Decrypt(byte[] encryptedData);
        string EncryptWithKey(string plainText, string key);
        string DecryptWithKey(string cipherText, string key);
        string GenerateKey();
        string GenerateIV();
    }
}
