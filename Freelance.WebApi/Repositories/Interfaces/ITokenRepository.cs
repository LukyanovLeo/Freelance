using Freelance.WebApi.Contracts.Entities;
using System;
using System.Threading.Tasks;

namespace Freelance.Repositories.Interfaces
{
    public interface ITokenRepository
    {
        Task CreateRefreshToken(RefreshToken refreshToken);
        Task UpdateRefreshToken(RefreshToken refreshToken);
        Task<RefreshToken> GetRefreshToken(Guid userId);
    }
}
