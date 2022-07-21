using Freelance.Services.Interfaces;
using Freelance.Utils;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.Security.Cryptography;
using System.Text;

namespace Freelance.Services
{
    public class HashService : IHashService
    {
        private const int PasswordLength = 32;

        public byte[] HashPassword(
            string password,
            byte[] salt,
            KeyDerivationPrf keyDerivationPrf = KeyDerivationPrf.HMACSHA1,
            int iterationCount = 10000)
        {
            return KeyDerivation.Pbkdf2(password, salt, keyDerivationPrf, iterationCount, PasswordLength);
        }

        public byte[] ComputeHash(string from, DigestAlgorith algorith)
        {
            var data = Encoding.UTF8.GetBytes(from);

            if (algorith == DigestAlgorith.Sha512)
            {
                using (SHA512 shaM = new SHA512Managed())
                {
                    return shaM.ComputeHash(data);
                }
            }

            return new byte[] { };
        }

        public bool VerifyPassword(
            string password,
            byte[] sessionSalt,
            byte[] hashedPassword,
            byte[] passwordSalt,
            KeyDerivationPrf keyDerivationPrf = KeyDerivationPrf.HMACSHA1,
            int iterationCount = 10000)
        {
            try
            {
                var providedPasswordBytes = Convert.FromBase64String(password);
                var reckonedPasswordBytes = HashPassword(Convert.ToBase64String(hashedPassword), sessionSalt, keyDerivationPrf, iterationCount);
                return ByteArraysUtils.Equal(providedPasswordBytes, reckonedPasswordBytes);
            }
            catch
            {
                return false;
            }
        }
    }

    public enum DigestAlgorith
    {
        Sha512 = 1
    }
}
