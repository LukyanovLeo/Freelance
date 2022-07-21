using Freelance.WebApi.Contracts.Common;
using Freelance.WebApi.Contracts.Responses;
using Freelance.Hubs.Interfaces;
using Freelance.QueryHandlers.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Freelance.Hubs
{
    [Authorize]
    public class MessageHub : Hub<IMessageHub>
    {
        private Guid UserId => Guid.Parse(Context?.User.Claims.Single(x => x.Type == JwtClaimTypes.UserId).Value);

        private readonly IChatQueryHandler _queryHandler;

        public MessageHub(IChatQueryHandler queryHandler)
        {
            _queryHandler = queryHandler;
        }

        public async Task CreateChat(Guid companionId)
        {
            await _queryHandler.CreateChat(companionId, UserId);
        }

        public async Task<GetChatsResponse> GetChats()
        {
            var result = await _queryHandler.GetAllChatsOfUser(UserId);
            return result;
        }

        public async Task<GetChatMessagesResponse> GetChatMessages(Guid chatId)
        {
            var result = await _queryHandler.GetChatMessages(chatId);
            return result;
        }

        public async Task SendMessage(string toUser, string message)
        {
            var userEmail = Context?.User.Claims.Single(x => x.Type == ClaimTypes.Email).Value;
            await Clients.User(toUser).ReceiveMessage(message, userEmail);
        }
    }
}
