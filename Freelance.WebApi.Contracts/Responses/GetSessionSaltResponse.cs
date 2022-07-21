using System;

namespace Freelance.WebApi.Contracts.Responses
{
    public class GetSessionSaltResponse
    {
        public string PasswordSaltAsBase64 { get; set; }
        public string SessionSaltAsBase64 { get; set; }
        public DateTime ExpiredAt { get; set; }
    }
}
