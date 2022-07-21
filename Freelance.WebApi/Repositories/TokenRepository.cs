using Dapper;
using Freelance.WebApi.Contracts.Entities;
using Freelance.Repositories.Interfaces;
using Freelance.Services.Interfaces;
using System;
using System.Threading.Tasks;

namespace Freelance.Repositories
{
    public class TokenRepository: ITokenRepository
    {
        private readonly IDapperService _dapper;

        public TokenRepository(IDapperService dapper)
        {
            _dapper = dapper;
        }

        public async Task CreateRefreshToken(RefreshToken user)
        {
            var query = @"INSERT INTO token.refresh_token (user_id, token, expires, created, created_by_ip) 
                        VALUES (@UserId, @Token, @Expires, @Created, @CreatedByIp)";
            await _dapper.Execute(query, user);
        }

        public async Task<RefreshToken> GetRefreshToken(Guid userId)
        {
            var query = @"SELECT * FROM token.refresh_token 
                        WHERE user_id = @UserId 
                        AND revoked IS NULL
                        ORDER BY created DESC
                        LIMIT 1";

            var parameters = new DynamicParameters();
            parameters.Add("UserId", userId);

            return await _dapper.QuerySingle<RefreshToken, DynamicParameters>(query, parameters);
        }

        public async Task UpdateRefreshToken(RefreshToken refreshToken)
        {
            var query = @"UPDATE token.refresh_token 
                        SET revoked = @Revoked,
                        revoked_by_ip = @RevokedByIp,
                        replaced_by_token = @ReplacedByToken 
                        WHERE user_id = @UserId
                        AND token = @Token";

            await _dapper.Execute(query, refreshToken);
        }
    }
}
