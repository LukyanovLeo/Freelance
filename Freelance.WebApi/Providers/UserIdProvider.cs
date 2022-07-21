using Freelance.WebApi.Contracts.Common;
using Microsoft.AspNetCore.SignalR;
using System.Linq;

namespace Freelance.Providers
{
    public class UserIdProvider : IUserIdProvider
    {
        public virtual string GetUserId(HubConnectionContext connection)
        {
            return connection?.User.Claims.Single(x => x.Type == JwtClaimTypes.UserId).Value;
        }
    }
}
