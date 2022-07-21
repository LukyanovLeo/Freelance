using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace Freelance.Services.Interfaces
{
    public interface IHashService
    {
        public byte[] HashPassword(string password, byte[] salt, KeyDerivationPrf keyDerivationPrf = KeyDerivationPrf.HMACSHA1, int iterationCount = 10000);
        public bool VerifyPassword(string password, byte[] sessionSalt, byte[] hashedPassword, byte[] passwordSalt, KeyDerivationPrf keyDerivationPrf = KeyDerivationPrf.HMACSHA1, int iterationCount = 10000);
        public byte[] ComputeHash(string from, DigestAlgorith algorith);
    }
}
