using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using IskoopDemo.Shared.Application.Interfaces.Cryptography;
using IskoopDemo.Shared.Models.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.Extensions.Options;

namespace IskoopDemo.Shared.Application.Services.Cryptography
{
    public class AesEncryptionService : IEncryptionService
    {
        private readonly EncryptionSettings _settings;
        private readonly byte[] _masterKey;

        public AesEncryptionService(IOptions<EncryptionSettings> settings)
        {
            _settings = settings.Value;
            if (string.IsNullOrEmpty(_settings.MasterKey))
            {
                throw new ArgumentException("Master key must be provided in settings.");
            }

            _masterKey = DeriveKey(_settings.MasterKey, GenerateSalt());
        }


        public string Encrypt(string plainText)
        {
            using var aes = Aes.Create();
            aes.Key = _masterKey;
            aes.GenerateIV();

            using var encryptor = aes.CreateEncryptor();
            using var ms = new MemoryStream();

            // Write IV to the beginning of the stream
            ms.Write(aes.IV, 0, aes.IV.Length);

            using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
            using (var sw = new StreamWriter(cs))
            {
                sw.Write(plainText);
            }

            return Convert.ToBase64String(ms.ToArray());

        }

        public string Decrypt(string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText))
                return null;

            var buffer = Convert.FromBase64String(cipherText);

            using var aes = Aes.Create();
            aes.Key = _masterKey;

            // Extract IV from the beginning of the buffer
            var iv = new byte[aes.BlockSize / 8];
            Array.Copy(buffer, 0, iv, 0, iv.Length);
            aes.IV = iv;

            using var decryptor = aes.CreateDecryptor();
            using var ms = new MemoryStream(buffer, iv.Length, buffer.Length - iv.Length);
            using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
            using var sr = new StreamReader(cs);

            return sr.ReadToEnd();
        }

        public byte[] Encrypt(byte[] data)
        {
            if (data == null || data.Length == 0)
                return null;

            using var aes = Aes.Create();
            aes.Key = _masterKey;
            aes.GenerateIV();

            using var encryptor = aes.CreateEncryptor();
            using var ms = new MemoryStream();

            // Write IV to the beginning
            ms.Write(aes.IV, 0, aes.IV.Length);

            using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
            {
                cs.Write(data, 0, data.Length);
            }

            return ms.ToArray();
        }

        public byte[] Decrypt(byte[] encryptedData)
        {
            if (encryptedData == null || encryptedData.Length == 0)
                return null;

            using var aes = Aes.Create();
            aes.Key = _masterKey;

            // Extract IV
            var iv = new byte[aes.BlockSize / 8];
            Array.Copy(encryptedData, 0, iv, 0, iv.Length);
            aes.IV = iv;

            using var decryptor = aes.CreateDecryptor();
            using var ms = new MemoryStream(encryptedData, iv.Length, encryptedData.Length - iv.Length);
            using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
            using var result = new MemoryStream();

            cs.CopyTo(result);
            return result.ToArray();
        }

        public string EncryptWithKey(string plainText, string key)
        {
            if (string.IsNullOrEmpty(plainText))
                return null;

            var keyBytes = DeriveKey(key, GenerateSalt());

            using var aes = Aes.Create();
            aes.Key = keyBytes;
            aes.GenerateIV();

            using var encryptor = aes.CreateEncryptor();
            using var ms = new MemoryStream();

            ms.Write(aes.IV, 0, aes.IV.Length);

            using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
            using (var sw = new StreamWriter(cs))
            {
                sw.Write(plainText);
            }

            return Convert.ToBase64String(ms.ToArray());
        }

        public string DecryptWithKey(string cipherText, string key)
        {
            if (string.IsNullOrEmpty(cipherText))
                return null;

            var keyBytes = DeriveKey(key, GenerateSalt());
            var buffer = Convert.FromBase64String(cipherText);

            using var aes = Aes.Create();
            aes.Key = keyBytes;

            var iv = new byte[aes.BlockSize / 8];
            Array.Copy(buffer, 0, iv, 0, iv.Length);
            aes.IV = iv;

            using var decryptor = aes.CreateDecryptor();
            using var ms = new MemoryStream(buffer, iv.Length, buffer.Length - iv.Length);
            using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
            using var sr = new StreamReader(cs);

            return sr.ReadToEnd();
        }

        public string GenerateKey()
        {
            using var aes = Aes.Create();
            aes.GenerateKey();
            return Convert.ToBase64String(aes.Key);
        }

        public string GenerateIV()
        {
            using var aes = Aes.Create();
            aes.GenerateIV();
            return Convert.ToBase64String(aes.IV);
        }

        private byte[] DeriveKey(string password, string salt)
        {
            return KeyDerivation.Pbkdf2(
                password: password,
                salt: Encoding.UTF8.GetBytes(salt),
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: _settings.Iterations,
                numBytesRequested: _settings.KeySize / 8
            );
        }

        private string GenerateSalt()
        {
            var salt = new byte[_settings.SaltSize / 8];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(salt);
            return Convert.ToBase64String(salt);
        }
    }
}
