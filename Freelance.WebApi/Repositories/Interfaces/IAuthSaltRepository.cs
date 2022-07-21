using Freelance.WebApi.Contracts.Entities;
using System;
using System.Threading.Tasks;

namespace Freelance.Repositories.Interfaces
{
    public interface IAuthSaltRepository
    {
        Task CreateAuthSalt(AuthSalt authSalt);
        Task<string> GetUserPasswordSalt(Guid userId);
    }
}
