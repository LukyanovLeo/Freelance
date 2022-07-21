using Freelance.WebApi.Contracts.Responses;
using System;
using System.Threading.Tasks;

namespace Freelance.QueryHandlers.Interfaces
{
    public interface IChatQueryHandler
    {
        Task CreateChat(Guid toUserId, Guid userId);
        Task<GetChatMessagesResponse> GetChatMessages(Guid chatId);
        Task<GetChatsResponse> GetAllChatsOfUser(Guid userId);
    }
}
