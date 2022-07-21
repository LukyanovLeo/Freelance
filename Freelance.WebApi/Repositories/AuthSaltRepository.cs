using Dapper;
using Freelance.WebApi.Contracts.Entities;
using Freelance.Repositories.Interfaces;
using Freelance.Services.Interfaces;
using System;
using System.Threading.Tasks;

namespace Freelance.Repositories
{
    public class AuthSaltRepository: IAuthSaltRepository
    {

        private readonly IDapperService _dapper;

        public AuthSaltRepository(IDapperService dapper)
        {
            _dapper = dapper;
        }

        public async Task CreateAuthSalt(AuthSalt authSalt)
        {
            var query = @"INSERT INTO security.auth_salt (user_id, salt) 
                        VALUES (@UserId, @Salt)";

            await _dapper.Execute(query, authSalt);
        }

        public async Task<string> GetUserPasswordSalt(Guid userId)
        {
            var query = @"SELECT salt
                            FROM security.auth_salt
                            WHERE user_id = @UserId";

            var parameters = new DynamicParameters();
            parameters.Add("UserId", userId);

            return await _dapper.QuerySingle<string, DynamicParameters>(query, parameters);
        }
    }
}
