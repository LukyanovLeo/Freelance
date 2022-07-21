using System;

namespace Freelance.WebApi.Contracts.Entities
{
    public class Attachment
    {
        public Guid Id { get; set; }
        public string Path { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
