using Freelance.WebApi.Contracts.DTO;
using System.Collections.Generic;

namespace Freelance.WebApi.Contracts.Responses
{
    public class GetChatMessagesResponse
    {
        public List<Message> Messages { get; set; }
    }
}
