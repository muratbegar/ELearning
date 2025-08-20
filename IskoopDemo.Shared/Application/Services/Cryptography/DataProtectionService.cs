using IskoopDemo.Shared.Application.Interfaces.Cryptography;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace IskoopDemo.Shared.Application.Services.Cryptography
{
    public class DataProtectionService : IDataProtectionService
    {
        private readonly IEncryptionService _encryptionService;
        private readonly IHashingService _hashingService;

        public DataProtectionService(IEncryptionService encryptionService, IHashingService hashingService)
        {
            _encryptionService = encryptionService ?? throw new ArgumentNullException(nameof(encryptionService));
            _hashingService = hashingService ?? throw new ArgumentNullException(nameof(hashingService));
        }

        public string Protect(string plainText, string purpose = null)
        {
            if (string.IsNullOrEmpty(plainText))
                return null;

            var encrypted = _encryptionService.Encrypt(plainText);

            if (!string.IsNullOrEmpty(purpose))
            {
                var hmac = _hashingService.ComputeHmac(encrypted, purpose);
                return $"{encrypted}.{hmac}";
            }

            return encrypted;
        }

        public byte[] Protect(byte[] plainData, string purpose = null)
        {
            if (plainData == null || plainData.Length == 0)
                return null;

            var encrypted = _encryptionService.Encrypt(plainData);

            if (!string.IsNullOrEmpty(purpose))
            {
                var hmac = _hashingService.ComputeHmac(Convert.ToBase64String(encrypted), purpose);
                var hmacBytes = Convert.FromBase64String(hmac);

                var result = new byte[encrypted.Length + hmacBytes.Length];
                Array.Copy(encrypted, 0, result, 0, encrypted.Length);
                Array.Copy(hmacBytes, 0, result, encrypted.Length, hmacBytes.Length);

                return result;
            }

            return encrypted;
        }

        public string Unprotect(string protectedText, string purpose = null)
        {
            if (string.IsNullOrEmpty(protectedText))
                return null;

            string encrypted;

            if (!string.IsNullOrEmpty(purpose))
            {
                var parts = protectedText.Split('.');
                if (parts.Length != 2)
                    throw new CryptographicException("Invalid protected data format");

                encrypted = parts[0];
                var hmac = parts[1];

                if (!_hashingService.VerifyHmac(encrypted, purpose, hmac))
                    throw new CryptographicException("Data integrity check failed");
            }
            else
            {
                encrypted = protectedText;
            }

            return _encryptionService.Decrypt(encrypted);
        }

        public byte[] Unprotect(byte[] protectedData, string purpose = null)
        {
            if (protectedData == null || protectedData.Length == 0)
                return null;

            byte[] encrypted;

            if (!string.IsNullOrEmpty(purpose))
            {
                var hmacSize = 32; // SHA256 size
                if (protectedData.Length < hmacSize)
                    throw new CryptographicException("Invalid protected data size");

                encrypted = new byte[protectedData.Length - hmacSize];
                var hmacBytes = new byte[hmacSize];

                Array.Copy(protectedData, 0, encrypted, 0, encrypted.Length);
                Array.Copy(protectedData, encrypted.Length, hmacBytes, 0, hmacSize);

                var hmac = Convert.ToBase64String(hmacBytes);
                if (!_hashingService.VerifyHmac(Convert.ToBase64String(encrypted), purpose, hmac))
                    throw new CryptographicException("Data integrity check failed");
            }
            else
            {
                encrypted = protectedData;
            }

            return _encryptionService.Decrypt(encrypted);
        }
    }
}
