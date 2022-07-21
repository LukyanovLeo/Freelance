using System;

namespace Freelance.WebApi.Contracts.Entities
{
    public class AuthSalt
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Salt { get; set; }
    }
}
