using System;

namespace Freelance.WebApi.Contracts.Entities
{
    public class Chat
    {
        public Guid Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid CreatedUserId { get; set; }
        public User User { get; set; }
        public string LastMessage { get; set; }
        public DateTime LastMessageDate { get; set; }
    }
}
