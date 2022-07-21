using System;

namespace Freelance.WebApi.Contracts.Entities
{
    public class Message
    {
        public Guid Id { get; set; }
        public Guid ChatId { get; set; }
        public Guid SenderId { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Text { get; set; }
    }
}
