using Freelance.WebApi.Contracts.Entities;
using System;
using System.Threading.Tasks;

namespace Freelance.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<bool> IsUserExist(string email);
        Task<bool> IsLoginExist(string login);
        Task<Guid> CreateUser(User user);
        Task<Guid> GetIdUserByEmail(string email);
        Task<User> GetUserByEmail(string email);
    }
}
