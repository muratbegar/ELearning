using IskoopDemo.Shared.Application.Interfaces.Cryptography;
using IskoopDemo.Shared.Models.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace IskoopDemo.Shared.Application.Services.Cryptography
{
    public class HashingService : IHashingService
    {

        private readonly HashingSettings _settings;

        public HashingService(IOptions<HashingSettings> settings)
        {
            _settings = settings.Value ?? throw new ArgumentNullException(nameof(settings), "Hashing settings cannot be null.");
        }

        public string Hash(string input)
        {
            if (string.IsNullOrEmpty(input))
                return null;

            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
            return Convert.ToBase64String(bytes);

        }

        public string HashWithSalt(string input, out string salt)
        {
            if (string.IsNullOrEmpty(input))
            {
                salt = null;
                return null;
            }

            salt = GenerateSalt();
            var hashed = KeyDerivation.Pbkdf2(
                password: input,
                salt: Convert.FromBase64String(salt),
                prf: _settings.Algorithm,
                iterationCount: _settings.Iterations,
                numBytesRequested: _settings.HashSize / 8
            );

            return Convert.ToBase64String(hashed);
        }

        public bool Verify(string input, string hash, string salt)
        {
            if (string.IsNullOrEmpty(input) || string.IsNullOrEmpty(hash) || string.IsNullOrEmpty(salt))
                return false;

            var hashed = KeyDerivation.Pbkdf2(
                password: input,
                salt: Convert.FromBase64String(salt),
                prf: _settings.Algorithm,
                iterationCount: _settings.Iterations,
                numBytesRequested: _settings.HashSize / 8
            );

            return Convert.ToBase64String(hashed) == hash;
        }

        public string GenerateSalt()
        {
            var salt = new byte[_settings.SaltSize / 8];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(salt);
            return Convert.ToBase64String(salt);
        }

        public string ComputeHmac(string data, string key)
        {
            if (string.IsNullOrEmpty(data) || string.IsNullOrEmpty(key))
                return null;

            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
            return Convert.ToBase64String(hash);
        }

        public bool VerifyHmac(string data, string key, string hmac)
        {
            if (string.IsNullOrEmpty(data) || string.IsNullOrEmpty(key) || string.IsNullOrEmpty(hmac))
                return false;

            var computed = ComputeHmac(data, key);
            return computed == hmac;
        }
    }
}
