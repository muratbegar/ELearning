using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ELearningIskoop.BuildingBlocks.Domain;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace ELearningIskoop.Shared.Domain.ValueObjects
{
    public class HashedPassword : ValueObject
    {
        private const int SaltSize = 128 / 8; //128bit
        private const int KeySize = 256 / 8; //256bit
        private const int Iterations = 10000;

        public string Hash { get; private set; }
        public string Salt { get; private set; }


        private HashedPassword(string hash, string salt)
        {
            Hash = hash;
            Salt = salt;
        }

        // Plain password'dan hashed password oluştur
        public static HashedPassword Create(string plainPassword)
        {
            if(string.IsNullOrWhiteSpace(plainPassword))
                throw new ArgumentNullException("Password cannot be empty");

            if (plainPassword.Length < 8)
                throw new ArgumentNullException("Password must be at least 8 characters");

            //Salt oluştur
            var salt = GenerateSalt();

            //Hash Oluştur
            var hash = HashPassword(plainPassword,salt);

            return new HashedPassword(
                Convert.ToBase64String(hash),
                Convert.ToBase64String(salt));


        }
        // Database'den gelen hash ve salt ile instance oluştur
        public static HashedPassword FromStorage(string hash, string salt)
        {
            return new HashedPassword(hash, salt);
        }

        // Password doğrulama
        public bool Verify(string plainPassword)
        {
            if (string.IsNullOrWhiteSpace(plainPassword))
                return false;

            var salt = Convert.FromBase64String(Salt);
            var hash = HashPassword(plainPassword, salt);
            var storedHash = Convert.FromBase64String(Hash);

            return SlowEquals(hash, storedHash);
        }

        private static byte[] GenerateSalt()
        {
            var salt = new byte[SaltSize];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(salt);
            return salt;
        }

        private static byte[] HashPassword(string password, byte[] salt)
        {
            return KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: Iterations,
                numBytesRequested: KeySize
            );
        }

        // Timing attack'lara karşı güvenli karşılaştırma
        private static bool SlowEquals(byte[] a, byte[] b)
        {
            var diff = (uint)a.Length - (uint)b.Length;
            for (var i = 0; i < a.Length && i < b.Length; i++)
            {
                diff |= (uint)(a[i] ^ b[i]);
            }
            return diff == 0;
        }
        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Hash;
            yield return Salt;
        }
    }


}
