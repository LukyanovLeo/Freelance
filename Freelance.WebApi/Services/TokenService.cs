using Freelance.WebApi.Contracts.Common;
using Freelance.WebApi.Contracts.Entities;
using Freelance.WebApi.Contracts.Settings;
using Freelance.Services.Interfaces;
using Freelance.Utils;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace Freelance.Services
{
    public class TokenService : ITokenService
    {

        private readonly AuthOptions _authOptions;

        public TokenService(IOptions<AuthOptions> authOptions)
        {
            _authOptions = authOptions.Value;
        }

        public string GenerateJWT(User user)
        {
            var securityKey = _authOptions.GetSymmetricSecurityKey();
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512);

            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(JwtClaimTypes.UserId, user.Id.ToString()),
            };

            var token = new JwtSecurityToken(_authOptions.Issuer,
                _authOptions.Audience,
                claims,
                expires: DateTime.Now.AddSeconds(_authOptions.TokenLifeTime),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateJWT(List<Claim> claims)
        {
            var securityKey = _authOptions.GetSymmetricSecurityKey();
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512);

            var token = new JwtSecurityToken(_authOptions.Issuer,
                _authOptions.Audience,
                claims,
                expires: DateTime.Now.AddSeconds(_authOptions.TokenLifeTime),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public RefreshToken GenerateRefreshToken(Guid userId, string ipAddress)
        {
            using (var rngCryptoServiceProvider = new RNGCryptoServiceProvider())
            {
                var randomBytes = new byte[64];
                rngCryptoServiceProvider.GetBytes(randomBytes);
                return new RefreshToken
                {
                    UserId = userId,
                    Token = Convert.ToBase64String(randomBytes),
                    Expires = DateTime.UtcNow.AddSeconds(_authOptions.RefreshTokenLifeTime),
                    Created = DateTime.UtcNow,
                    CreatedByIp = ipAddress
                };
            }
        }
    }
}
