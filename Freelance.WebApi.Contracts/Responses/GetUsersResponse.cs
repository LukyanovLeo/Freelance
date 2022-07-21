using System.Collections.Generic;

namespace Freelance.WebApi.Contracts.Responses
{
    public class GetUsersResponse
    {
        public List<Contracts.DTO.User> Users { get; set; }
    }
}
