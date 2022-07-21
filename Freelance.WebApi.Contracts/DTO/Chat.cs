using System;

namespace Freelance.WebApi.Contracts.DTO
{
    public class Chat
    {
        public Guid Id { get; set; }
        public User User { get; set; }
        public string LastMessage { get; set; }
        public DateTime LastMessageDate { get; set; }
    }
}
