using Freelance.WebApi.Contracts.Responses;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Freelance.QueryHandlers.Interfaces
{
    public interface IAuthQueryHandler
    {
        Task<LoginResponse> Login(string email, string ipAddress);
        Task<GetSessionSaltResponse> GetSessionSalt(string email);
        string GetPublicKey();
        Task<RegistrationResponse> Registration(string email, string password, string ipAddress);
        Task<RefreshTokenResponse> RefreshToken(Guid userId, string refreshToken, List<Claim> UserClaims, string ipAddress);
        Task<bool> RevokeToken(Guid userId, string ipAddress);
    }
}
