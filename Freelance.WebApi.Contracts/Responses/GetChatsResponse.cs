using Freelance.WebApi.Contracts.DTO;
using System.Collections.Generic;

namespace Freelance.WebApi.Contracts.Responses
{
    public class GetChatsResponse
    {
        public List<Chat> Chats { get; set; }
    }
}
