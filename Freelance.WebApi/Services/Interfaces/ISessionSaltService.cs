using Freelance.WebApi.Contracts.Responses;
using System;

namespace Freelance.Services.Interfaces
{
    public interface ISessionSaltService
    {
        public GetSessionSaltResponse GenerateSalt(byte[] passwordSalt);
        public bool IsValid(byte[] salt, DateTime expiredAt);
    }
}
