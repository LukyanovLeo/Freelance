using System;

namespace Freelance.WebApi.Contracts.Requests
{
    public class LoginRequest
    {
        public string Email { get; set; }
        public string HashedPassword { get; set; }
        public string SessionSaltAsBase64 { get; set; }
        public DateTime ExpiredAt { get; set; }
    }
}
