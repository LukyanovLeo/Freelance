using Freelance.WebApi.Contracts;
using Freelance.WebApi.Contracts.Responses;
using Freelance.WebApi.Contracts.Settings;
using Freelance.Repositories.Interfaces;
using Freelance.Services.Interfaces;
using Freelance.Utils;
using Microsoft.Extensions.Options;
using System;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Freelance.Services
{
    public class SessionSaltService : ISessionSaltService
    {
        private const string SessionStringTemplate = "{0}{1}";

        private readonly AuthOptions _authOptions;
        private readonly IHashService _hashService;

        public SessionSaltService(IOptions<AuthOptions> authOptions, IHashService hashService)
        {
            _authOptions = authOptions.Value;
            _hashService = hashService;
        }

        public bool IsValid(byte[] salt, DateTime expiredAt)
        {
            if (DateTime.UtcNow > expiredAt)
            {
                return false;
            }

            var sessionSalt = CreateSessionSalt(expiredAt);

            return ByteArraysUtils.Equal(salt, sessionSalt);
        }

        public GetSessionSaltResponse GenerateSalt(byte[] passwordSalt)
        {
            var expiredAt = DateTime.UtcNow.AddSeconds(_authOptions.AuthSaltRequestLifeTime);
            var sessionSalt = CreateSessionSalt(expiredAt);

            return new GetSessionSaltResponse
            {
                PasswordSaltAsBase64 = Convert.ToBase64String(passwordSalt),
                SessionSaltAsBase64 = Convert.ToBase64String(sessionSalt),
                ExpiredAt = expiredAt
            };
        }

        private byte[] CreateSessionSalt(DateTime expiredAt)
        {
            var sessionSaltSecureString = string.Format(
                SessionStringTemplate,
                _authOptions.SecureRandomString,
                expiredAt.Ticks);


            return _hashService.ComputeHash(sessionSaltSecureString, DigestAlgorith.Sha512);
        }
    }
}
