using Freelance.WebApi.Contracts;
using Freelance.WebApi.Contracts.Entities;
using Freelance.WebApi.Contracts.Responses;
using Freelance.WebApi.Contracts.Settings;
using Freelance.QueryHandlers.Interfaces;
using Freelance.Repositories.Interfaces;
using Freelance.Services;
using Freelance.Services.Interfaces;
using Freelance.Utils;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Freelance.QueryHandlers
{
    public class AuthQueryHandler : IAuthQueryHandler
    {
        private readonly AuthOptions _authOptions;
        private readonly ITokenService _tokenService;
        private readonly ICertificateService _certificateService;
        private readonly IUserRepository _userRepository;
        private readonly ISessionSaltService _sessionSaltService;
        private readonly IHashService _hashService;
        private readonly ITokenRepository _tokenRepository;
        private readonly IAuthSaltRepository _authSaltRepository;

        public AuthQueryHandler(
            IOptions<AuthOptions> authOptions,
            ITokenService tokenService,
            ICertificateService certificateService,
            IUserRepository userRepository,
            ISessionSaltService sessionSaltService,
            IHashService hashService,
            ITokenRepository tokenRepository,
            IAuthSaltRepository authSaltRepository
            )
        {
            _authOptions = authOptions.Value;
            _tokenService = tokenService;
            _certificateService = certificateService;
            _userRepository = userRepository;
            _sessionSaltService = sessionSaltService;
            _hashService = hashService;
            _tokenRepository = tokenRepository;
            _authSaltRepository = authSaltRepository;
        }

        public async Task<LoginResponse> Login(string email, string ipAddress)
        {
            var user = await _userRepository.GetUserByEmail(email);
            var refreshToken = _tokenService.GenerateRefreshToken(user.Id, ipAddress);

            await _tokenRepository.CreateRefreshToken(refreshToken);

            return new LoginResponse
            {
                AccessToken = _tokenService.GenerateJWT(user),
                RefreshToken = refreshToken.Token
            };
        }

        public async Task<RegistrationResponse> Registration(string email, string password, string ipAddress)
        {
            var cert = _certificateService.LoadCertificate(_authOptions.CertificateSubject);
            var passwordDecrypted = _certificateService.TryDecrypt(cert, password);

            if (passwordDecrypted == null)
                throw new ApplicationException("При расшифровки пароля произошла ошибка");

            var passwordSalt = RandomGenerator.GenerateBytes();
            var passwordHash = _hashService.HashPassword(passwordDecrypted, passwordSalt);
            password = Convert.ToBase64String(passwordHash);

            var login = email.Split('@')[0];
            while (await _userRepository.IsLoginExist(login))
            {
                login = $"{login}{new Random().Next(1, 10001)}";
            }

            var user = new User
            {
                Email = email,
                Password = password,
                Login = login,
                RegistrationDate = DateTime.Now
            };

            var id = await _userRepository.CreateUser(user);
            user.Id = id;

            await _authSaltRepository.CreateAuthSalt(new AuthSalt
            {
                UserId = id,
                Salt = Convert.ToBase64String(passwordSalt)
            });

            var refreshToken = _tokenService.GenerateRefreshToken(user.Id, ipAddress);
            await _tokenRepository.CreateRefreshToken(refreshToken);

            return new RegistrationResponse
            {
                AccessToken = _tokenService.GenerateJWT(user),
                RefreshToken = refreshToken.Token
            };
        }

        public async Task<RefreshTokenResponse> RefreshToken(Guid userId, string refreshToken, List<Claim> userClaims, string ipAddress)
        {
            if (userId == null || userId == Guid.Empty)
                throw new ApplicationException("Пользоваиель не найден");

            var tokenInfo = await _tokenRepository.GetRefreshToken(userId);

            if (tokenInfo.Token != refreshToken || !tokenInfo.IsActive)
                throw new SecurityTokenException("Невалидный токен");

            var newRefreshToken = _tokenService.GenerateRefreshToken(userId, ipAddress);
            tokenInfo.Revoked = DateTime.UtcNow;
            tokenInfo.RevokedByIp = ipAddress;
            tokenInfo.ReplacedByToken = newRefreshToken.Token;

            await _tokenRepository.CreateRefreshToken(newRefreshToken);
            await _tokenRepository.UpdateRefreshToken(tokenInfo);

            return new RefreshTokenResponse
            {
                AccessToken = _tokenService.GenerateJWT(userClaims),
                RefreshToken = newRefreshToken.Token
            };
        }

        public async Task<bool> RevokeToken(Guid userId, string ipAddress)
        {
            if (userId == null || userId == Guid.Empty)
                throw new ApplicationException("Пользоваиель не найден");

            var refreshToken = await _tokenRepository.GetRefreshToken(userId);

            if (!refreshToken.IsActive)
                return false;

            refreshToken.Revoked = DateTime.UtcNow;
            refreshToken.RevokedByIp = ipAddress;

            await _tokenRepository.UpdateRefreshToken(refreshToken);

            return true;
        }

        public async Task<GetSessionSaltResponse> GetSessionSalt(string email)
        {
            var userId = await _userRepository.GetIdUserByEmail(email);
            var salt = await _authSaltRepository.GetUserPasswordSalt(userId);
            return _sessionSaltService.GenerateSalt(Convert.FromBase64String(salt));
        }

        public string GetPublicKey()
        {
            var cert = _certificateService.LoadCertificate(_authOptions.CertificateSubject);
            return _certificateService.GetPublicKey(cert);
        }
    }
}
