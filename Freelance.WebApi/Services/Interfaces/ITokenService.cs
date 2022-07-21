using Freelance.WebApi.Contracts.Entities;
using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace Freelance.Services.Interfaces
{
    public interface ITokenService
    {
        string GenerateJWT(User user);
        string GenerateJWT(List<Claim> claims);
        RefreshToken GenerateRefreshToken(Guid userId, string ipAddress);
    }
}
