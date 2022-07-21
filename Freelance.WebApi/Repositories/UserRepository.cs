using Dapper;
using Freelance.WebApi.Contracts.Entities;
using Freelance.Repositories.Interfaces;
using Freelance.Services.Interfaces;
using System;
using System.Threading.Tasks;

namespace Freelance.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IDapperService _dapper;

        public UserRepository(IDapperService dapper)
        {
            _dapper = dapper;
        }

        public async Task<bool> IsUserExist(string email)
        {
            var query = @"SELECT CASE WHEN EXISTS (
                            SELECT*
                            FROM public.user
                            WHERE email = @Email
                          )
                          THEN CAST(1 AS BIT)
                          ELSE CAST(0 AS BIT) END";

            var parameters = new DynamicParameters();
            parameters.Add("Email", email);

            return await _dapper.QuerySingle<bool, DynamicParameters>(query, parameters);
        }

        public async Task<bool> IsLoginExist(string login)
        {
            var query = @"SELECT CASE WHEN EXISTS (
                            SELECT 1
                            FROM public.user
                            WHERE login = @Login
                          )
                          THEN CAST(1 AS BIT)
                          ELSE CAST(0 AS BIT) END AS is_login_exist";

            var parameters = new DynamicParameters();
            parameters.Add("Login", login);

            return await _dapper.QuerySingle<bool, DynamicParameters>(query, parameters);
        }

        public async Task<Guid> CreateUser(User user)
        {
            var query = "INSERT INTO public.user (email, password, login, registration_date) VALUES (@Email, @Password, @Login, @RegistrationDate) RETURNING id";

            return await _dapper.QuerySingle<Guid, User>(query, user);
        }

        public async Task<Guid> GetIdUserByEmail(string email)
        {
            var query = @"SELECT id
                            FROM public.user
                            WHERE email = @Email";

            var parameters = new DynamicParameters();
            parameters.Add("Email", email);

            return await _dapper.QuerySingle<Guid, DynamicParameters>(query, parameters);
        }

        public async Task<User> GetUserByEmail(string email)
        {
            var query = @"SELECT *
                            FROM public.user
                            WHERE email = @Email";

            var parameters = new DynamicParameters();
            parameters.Add("Email", email);

            return await _dapper.QuerySingle<User, DynamicParameters>(query, parameters);
        }
    }
}
