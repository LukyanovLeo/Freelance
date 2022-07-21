using Dapper;
using System;
using System.Data;
using System.Data.Freelance.WebApi.Common;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using Npgsql;
using System.Collections.Generic;
using Freelance.Services.Interfaces;
using Freelance.WebApi.Contracts.Settings;

namespace Freelance.Services
{
    public class DapperService : IDapperService
    {
        private readonly string _connectionString;

        public DapperService(IOptions<AppSettings> appSettings)
        {
            Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

            _connectionString = appSettings.Value.ConnectionString;
        }

        private DbConnection GetDbconnection()
        {
            return new NpgsqlConnection(_connectionString);
        }


        public async Task<T> QuerySingle<T, D>(string sql, D entity, CommandType commandType = CommandType.Text)
        {
            try
            {
                using (var connection = GetDbconnection())
                    return await connection.QuerySingleAsync<T>(sql, entity, commandType: commandType);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task Execute<T>(string sql, T entity, CommandType commandType = CommandType.Text)
        {
            try
            {
                using (var connection = GetDbconnection())
                    await connection.ExecuteAsync(sql, entity, commandType: commandType);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<IEnumerable<T>> Query<T>(string sql, DynamicParameters dp = null, CommandType commandType = CommandType.Text)
        {
            try
            {
                using (var connection = GetDbconnection())
                    return await connection.QueryAsync<T>(sql, dp, commandType: commandType);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<IEnumerable<TReturn>> Query<TFirst, TSecond, TReturn>(string sql, Func<TFirst, TSecond, TReturn> map, DynamicParameters dp = null, string splitOn = "id", CommandType commandType = CommandType.Text)
        {
            try
            {
                using (var connection = GetDbconnection())
                    return await connection.QueryAsync<TFirst, TSecond, TReturn>(sql, map, dp, splitOn: splitOn);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
    }
}