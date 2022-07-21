using System;

namespace Freelance.WebApi.Contracts.Entities
{
    public class Party
    {
        public Guid ChatId { get; set; }
        public Guid UserId { get; set; }
    }
}
