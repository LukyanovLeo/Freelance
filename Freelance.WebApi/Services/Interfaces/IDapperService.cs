using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Freelance.Services.Interfaces
{
    public interface IDapperService
    {
        Task<T> QuerySingle<T, D>(string sql, D entity, CommandType commandType = CommandType.Text);
        Task Execute<T>(string sql, T entity, CommandType commandType = CommandType.Text);
        Task<IEnumerable<T>> Query<T>(string sql, DynamicParameters dp = null, CommandType commandType = CommandType.Text);
        Task<IEnumerable<TReturn>> Query<TFirst, TSecond, TReturn>(string sql, Func<TFirst, TSecond, TReturn> map, DynamicParameters dp = null, string splitOn = "id", CommandType commandType = CommandType.Text);
    }
}
