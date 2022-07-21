using System.Security.Cryptography;

namespace Freelance.Utils
{
    public static class RandomGenerator
    {
        public static byte[] GenerateBytes(int bytes = 100)
        {
            var salt = new byte[bytes];
            using(var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            return salt;
        }
    }
}
