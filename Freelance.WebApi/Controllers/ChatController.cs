using Freelance.WebApi.Contracts.Common;
using Freelance.WebApi.Contracts.Requests;
using Freelance.WebApi.Contracts.Responses;
using Freelance.QueryHandlers.Interfaces;
using Freelance.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Freelance.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly IDapperService _dapper;

        public ChatController(IDapperService dapper)
        {
            _dapper = dapper;
        }

        [Authorize]
        [HttpPost("login")]
        public async Task<GetUsersResponse> GetUsers()
        {
            var query = @"SELECT *
                          FROM public.user";

            var result = await _dapper.Query<Contracts.Entities.User>(query);

            return new GetUsersResponse
            {
                Users = result.Select(x => new Contracts.DTO.User
                {
                    Email = x.Email,
                    Id = x.Id
                }).ToList()
            };
        }
    }
}
