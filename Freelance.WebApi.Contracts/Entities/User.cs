using System;

namespace Freelance.WebApi.Contracts.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Login { get; set; }
        public DateTime RegistrationDate { get; set; }
    }
}