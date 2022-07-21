using Freelance.WebApi.Contracts.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Freelance.Repositories.Interfaces
{
    public interface IChatRepository
    {
        Task<Guid> CreateChat(Guid userId);
        Task CreateParty(List<Party> party);
        Task CreateMessage(Guid chatId, string text);
        Task<Guid> CreateAttachment(string filepath);
        Task<IEnumerable<Chat>> GetChatsByUserId(Guid userId);
        Task<IEnumerable<Message>> GetChatMessages(Guid chatId);
    }
}
